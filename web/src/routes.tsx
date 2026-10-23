import { Navigate, createBrowserRouter } from 'react-router-dom'
import ClienteLayout from './layouts/ClienteLayout'
import OperacaoLayout from './layouts/OperacaoLayout'
import EntregadorLayout from './layouts/EntregadorLayout'
import RequireRole from './components/RequireRole'
import Placeholder from './pages/Placeholder'
import Offline from './pages/Offline'
import NaoEncontrada from './pages/NaoEncontrada'
import Ping from './pages/Ping'

export const router = createBrowserRouter([
  { path: '/', element: <Navigate to="/app" replace /> },
  { path: '/login', element: <Placeholder titulo="Entrar (equipe)" descricao="Login da equipe: Etapa 1." /> },
  { path: '/entrar', element: <Placeholder titulo="Entrar (cliente)" descricao="Acesso por código (OTP): Etapa 1." /> },
  {
    path: '/app',
    element: <ClienteLayout />,
    children: [
      { index: true, element: <Placeholder titulo="Vitrine" /> },
      { path: 'encomendas', element: <Placeholder titulo="Minhas encomendas" /> },
      { path: 'perfil', element: <Placeholder titulo="Perfil" /> },
      { path: 'ping', element: <Ping /> },
    ],
  },
  {
    element: <OperacaoLayout />,
    children: [
      {
        path: '/pdv',
        element: (
          <RequireRole roles={['Atendente', 'Gerente', 'Admin']}>
            <Placeholder titulo="PDV" descricao="Em construção" />
          </RequireRole>
        ),
      },
      {
        path: '/kds',
        element: (
          <RequireRole roles={['Confeiteiro', 'Gerente', 'Admin']}>
            <Placeholder titulo="Cozinha" descricao="Em construção" />
          </RequireRole>
        ),
      },
      {
        path: '/admin',
        element: (
          <RequireRole roles={['Gerente', 'Admin']}>
            <Placeholder titulo="Administração" descricao="Em construção" />
          </RequireRole>
        ),
      },
    ],
  },
  {
    path: '/entrega',
    element: <EntregadorLayout />,
    children: [
      {
        index: true,
        element: (
          <RequireRole roles={['Entregador', 'Gerente', 'Admin']}>
            <Placeholder titulo="Entregas" descricao="Em construção" />
          </RequireRole>
        ),
      },
    ],
  },
  { path: '/offline', element: <Offline /> },
  { path: '*', element: <NaoEncontrada /> },
])
