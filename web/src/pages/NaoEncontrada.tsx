import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-3 p-6 text-center">
      <h1 className="text-2xl font-bold">Página não encontrada</h1>
      <p className="text-muted">O endereço acessado não existe.</p>
      <Link to="/app" className="font-medium text-accent underline">
        Voltar para a vitrine
      </Link>
    </div>
  )
}
