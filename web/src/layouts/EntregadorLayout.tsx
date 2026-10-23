import { Link, Outlet } from 'react-router-dom'
import Button from '../components/ui/Button'

export default function EntregadorLayout() {
  return (
    <div className="mx-auto flex min-h-screen max-w-xl flex-col">
      <header className="border-b border-line bg-surface px-4 py-3">
        <span className="text-lg font-bold text-accent">DoceFlow · Entregas</span>
      </header>
      <main className="flex-1 p-4">
        <Outlet />
      </main>
      <footer className="grid grid-cols-2 gap-3 border-t border-line bg-surface p-4 pb-[calc(1rem+env(safe-area-inset-bottom))]">
        <Link to="/entrega">
          <Button size="lg" className="w-full">
            Minhas entregas
          </Button>
        </Link>
        <Link to="/offline">
          <Button size="lg" variant="secondary" className="w-full">
            Ajuda
          </Button>
        </Link>
      </footer>
    </div>
  )
}
