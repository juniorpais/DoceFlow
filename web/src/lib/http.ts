const TOKEN_KEY = 'doceflow.token'
const BASE_URL = (import.meta.env.VITE_API_URL ?? '/api').replace(/\/+$/, '')

let memoryToken: string | null = null

export function setToken(token: string | null): void {
  memoryToken = token
  try {
    if (token) localStorage.setItem(TOKEN_KEY, token)
    else localStorage.removeItem(TOKEN_KEY)
  } catch {
    // armazenamento indisponível: mantém apenas em memória
  }
}

export function getToken(): string | null {
  if (memoryToken) return memoryToken
  try {
    memoryToken = localStorage.getItem(TOKEN_KEY)
  } catch {
    memoryToken = null
  }
  return memoryToken
}

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  readonly status: number
  readonly title: string
  readonly detail?: string
  readonly errors?: Record<string, string[]>
  readonly traceId?: string

  constructor(status: number, title: string, detail?: string, errors?: Record<string, string[]>, traceId?: string) {
    super(detail ?? title)
    this.name = 'ApiError'
    this.status = status
    this.title = title
    this.detail = detail
    this.errors = errors
    this.traceId = traceId
  }
}

export interface RequestOptions {
  body?: unknown
  signal?: AbortSignal
  headers?: Record<string, string>
  /** Gera um header Idempotency-Key (UUID) para POST. Informe uma string para reutilizar uma chave. */
  idempotent?: boolean | string
}

type Method = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

async function parseProblem(res: Response): Promise<ApiError> {
  let problem: ProblemDetails = {}
  try {
    problem = (await res.json()) as ProblemDetails
  } catch {
    // corpo vazio ou não-JSON
  }
  return new ApiError(
    res.status,
    problem.title ?? res.statusText ?? 'Erro na requisição',
    problem.detail,
    problem.errors,
    problem.traceId,
  )
}

async function request<T>(method: Method, path: string, opts: RequestOptions = {}): Promise<T> {
  const headers: Record<string, string> = { Accept: 'application/json', ...opts.headers }
  const token = getToken()
  if (token) headers.Authorization = `Bearer ${token}`
  if (opts.body !== undefined) headers['Content-Type'] = 'application/json'
  if (method === 'POST' && opts.idempotent) {
    headers['Idempotency-Key'] = typeof opts.idempotent === 'string' ? opts.idempotent : crypto.randomUUID()
  }

  let res: Response
  try {
    res = await fetch(`${BASE_URL}${path.startsWith('/') ? path : `/${path}`}`, {
      method,
      headers,
      body: opts.body !== undefined ? JSON.stringify(opts.body) : undefined,
      signal: opts.signal,
    })
  } catch (e) {
    if (e instanceof DOMException && e.name === 'AbortError') throw e
    throw new ApiError(0, 'Sem conexão', 'Não foi possível falar com o servidor.')
  }

  if (!res.ok) throw await parseProblem(res)
  if (res.status === 204) return undefined as T
  const text = await res.text()
  if (!text) return undefined as T
  const contentType = res.headers.get('Content-Type') ?? ''
  return (contentType.includes('json') ? JSON.parse(text) : text) as T
}

export const http = {
  get: <T>(path: string, opts?: Omit<RequestOptions, 'body'>) => request<T>('GET', path, opts),
  post: <T>(path: string, body?: unknown, opts?: Omit<RequestOptions, 'body'>) => request<T>('POST', path, { ...opts, body }),
  put: <T>(path: string, body?: unknown, opts?: Omit<RequestOptions, 'body'>) => request<T>('PUT', path, { ...opts, body }),
  patch: <T>(path: string, body?: unknown, opts?: Omit<RequestOptions, 'body'>) => request<T>('PATCH', path, { ...opts, body }),
  delete: <T>(path: string, opts?: Omit<RequestOptions, 'body'>) => request<T>('DELETE', path, opts),
}
