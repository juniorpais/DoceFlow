import Card from '../components/ui/Card'

interface Props {
  titulo: string
  descricao?: string
}

export default function Placeholder({ titulo, descricao = 'Em construção.' }: Props) {
  return (
    <Card>
      <h1 className="text-xl font-semibold">{titulo}</h1>
      <p className="mt-2 text-muted">{descricao}</p>
    </Card>
  )
}
