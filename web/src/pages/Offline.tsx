import Button from '../components/ui/Button'

export default function Offline() {
  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col items-center justify-center gap-4 p-6 text-center">
      <h1 className="text-2xl font-bold">Você está offline</h1>
      <p className="text-muted">Sem conexão no momento. O que já foi carregado continua disponível e suas ações pendentes serão enviadas quando a internet voltar.</p>
      <Button onClick={() => window.location.reload()}>Tentar novamente</Button>
    </div>
  )
}
