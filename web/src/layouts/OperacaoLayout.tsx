import { NavLink, Outlet } from 'react-router-dom'

const itens = [
  { to: '/pdv', label: 'PDV' },
  { to: '/kds', label: 'Cozinha' },
  { to: '/admin', label: 'Admin' },
]

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `flex flex-1 items-center justify-center rounded-lg px-3 py-3 text-sm font-medium lg:flex-none lg:justify-start ${
    isActive ? 'bg-accent text-accent-fg' : 'text-fg hover:bg-line/40'
  }`

export default function OperacaoLayout() {
  return (
    <div className="flex min-h-screen flex-col lg:flex-row">
      <aside className="hidden w-56 shrink-0 flex-col gap-1 border-r border-line bg-surface p-4 lg:flex">
        <span className="mb-4 text-lg font-bold text-accent">DoceFlow</span>
        <nav aria-label="Operação" className="flex flex-col gap-1">
          {itens.map((i) => (
            <NavLink key={i.to} to={i.to} className={linkClass}>
              {i.label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="flex-1 p-4 pb-24 lg:p-6 lg:pb-6">
        <Outlet />
      </main>
      <nav
        aria-label="Operação"
        className="fixed inset-x-0 bottom-0 z-10 flex gap-1 border-t border-line bg-surface p-1 lg:hidden"
      >
        {itens.map((i) => (
          <NavLink key={i.to} to={i.to} className={linkClass}>
            {i.label}
          </NavLink>
        ))}
      </nav>
    </div>
  )
}
