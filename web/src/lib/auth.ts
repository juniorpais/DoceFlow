import { useSyncExternalStore } from 'react'
import { z } from 'zod'
import { getToken, setToken } from './http'

export const ROLES = ['Cliente', 'Atendente', 'Confeiteiro', 'Entregador', 'Gerente', 'Admin'] as const
export type Role = (typeof ROLES)[number]

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

const claimsSchema = z
  .object({
    sub: z.string().optional(),
    name: z.string().optional(),
    exp: z.number().optional(),
    role: z.union([z.string(), z.array(z.string())]).optional(),
    roles: z.union([z.string(), z.array(z.string())]).optional(),
    [ROLE_CLAIM]: z.union([z.string(), z.array(z.string())]).optional(),
  })
  .passthrough()

export interface Session {
  token: string
  sub?: string
  name?: string
  exp?: number
  roles: Role[]
}

function base64UrlDecode(input: string): string {
  const b64 = input.replace(/-/g, '+').replace(/_/g, '/')
  const padded = b64 + '='.repeat((4 - (b64.length % 4)) % 4)
  const bytes = Uint8Array.from(atob(padded), (c) => c.charCodeAt(0))
  return new TextDecoder().decode(bytes)
}

function toList(v: string | string[] | undefined): string[] {
  if (v === undefined) return []
  return Array.isArray(v) ? v : [v]
}

/** Decodifica as claims do JWT SEM verificar a assinatura (apenas para UX; a API é a autoridade). */
export function decodeToken(token: string): Session | null {
  try {
    const payload = token.split('.')[1]
    if (!payload) return null
    const claims = claimsSchema.parse(JSON.parse(base64UrlDecode(payload)))
    const rawRoles = [...toList(claims.role), ...toList(claims.roles), ...toList(claims[ROLE_CLAIM])]
    const roles = rawRoles.filter((r): r is Role => (ROLES as readonly string[]).includes(r))
    return { token, sub: claims.sub, name: claims.name, exp: claims.exp, roles }
  } catch {
    return null
  }
}

function load(): Session | null {
  const token = getToken()
  if (!token) return null
  const session = decodeToken(token)
  if (!session) return null
  if (session.exp !== undefined && session.exp * 1000 < Date.now()) return null
  return session
}

let current: Session | null = load()
const listeners = new Set<() => void>()

function emit() {
  listeners.forEach((l) => l())
}

export function signIn(token: string): Session | null {
  const session = decodeToken(token)
  if (!session) return null
  setToken(token)
  current = session
  emit()
  return session
}

export function signOut(): void {
  setToken(null)
  current = null
  emit()
}

export function getSession(): Session | null {
  return current
}

function subscribe(listener: () => void): () => void {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

export function useSession(): Session | null {
  return useSyncExternalStore(subscribe, getSession, getSession)
}
