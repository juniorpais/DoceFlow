import { NavLink, Outlet } from 'react-router-dom'

const itens = [
  { to: '/app', label: 'Vitrine', end: true },
  { to: '/app/encomendas', label: 'Encomendas', end: false },
  { to: '/app/perfil', label: 'Perfil', end: false },
]

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `flex flex-1 flex-col items-center justify-center gap-1 py-3 text-xs font-medium lg:flex-none lg:px-3 lg:py-2 lg:text-sm ${
    isActive ? 'text-accent' : 'text-muted'
  }`

export default function ClienteLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <header className="sticky top-0 z-10 hidden border-b border-line bg-surface lg:block">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-3">
          <span className="text-lg font-bold text-accent">DoceFlow</span>
          <nav className="flex gap-2">
            {itens.map((i) => (
              <NavLink key={i.to} to={i.to} end={i.end} className={linkClass}>
                {i.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>
      <main className="mx-auto w-full max-w-5xl flex-1 px-4 pb-24 pt-4 lg:px-6 lg:pb-8">
        <Outlet />
      </main>
      <nav
        aria-label="Navegação principal"
        className="fixed inset-x-0 bottom-0 z-10 flex border-t border-line bg-surface pb-[env(safe-area-inset-bottom)] lg:hidden"
      >
        {itens.map((i) => (
          <NavLink key={i.to} to={i.to} end={i.end} className={linkClass}>
            {i.label}
          </NavLink>
        ))}
      </nav>
    </div>
  )
}
