import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useSession, type Role } from '../lib/auth'

interface Props {
  roles: Role[]
  /** Rota de login quando não há sessão. Padrão: /login (equipe) ou /entrar (cliente). */
  loginPath?: string
  children: ReactNode
}

export default function RequireRole({ roles, loginPath, children }: Props) {
  const session = useSession()
  const location = useLocation()

  if (!session) {
    const destino = loginPath ?? (roles.includes('Cliente') ? '/entrar' : '/login')
    return <Navigate to={destino} replace state={{ from: location.pathname }} />
  }

  const permitido = session.roles.some((r) => roles.includes(r) || r === 'Admin')
  if (!permitido) {
    return (
      <div className="mx-auto max-w-md p-8 text-center">
        <h1 className="text-xl font-semibold">Acesso negado</h1>
        <p className="mt-2 text-muted">Você não tem permissão para acessar esta área.</p>
      </div>
    )
  }

  return <>{children}</>
}
