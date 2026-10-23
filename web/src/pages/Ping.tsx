import { useQuery } from '@tanstack/react-query'
import Card from '../components/ui/Card'
import Button from '../components/ui/Button'
import { ApiError, http } from '../lib/http'

export default function Ping() {
  const { data, error, isFetching, refetch } = useQuery({
    queryKey: ['sistema', 'ping'],
    queryFn: () => http.get<unknown>('/v1/sistema/ping'),
  })

  return (
    <Card>
      <h1 className="text-xl font-semibold">Ping da API</h1>
      <p className="mt-1 text-sm text-muted">GET /api/v1/sistema/ping</p>
      {isFetching && <p className="mt-3">Consultando…</p>}
      {error && (
        <p className="mt-3 text-red-600" role="alert">
          {error instanceof ApiError ? `${error.title} (${error.status})` : 'Erro inesperado'}
        </p>
      )}
      {data !== undefined && (
        <pre className="mt-3 overflow-auto rounded-lg bg-bg p-3 text-sm">{JSON.stringify(data, null, 2)}</pre>
      )}
      <Button className="mt-4" onClick={() => void refetch()}>
        Consultar novamente
      </Button>
    </Card>
  )
}
