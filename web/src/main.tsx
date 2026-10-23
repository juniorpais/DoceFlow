import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { registerSW } from 'virtual:pwa-register'
import App from './App'
import './styles/index.css'

registerSW({ immediate: true })

const container = document.getElementById('root')
if (!container) throw new Error('Elemento #root não encontrado')

createRoot(container).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
