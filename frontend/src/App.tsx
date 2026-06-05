import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import Sidebar from './components/Sidebar'
import DashboardPage from './pages/DashboardPage'
import LocalizacoesPage from './pages/LocalizacoesPage'
import RecebimentoPage from './pages/RecebimentoPage'
import SeparacaoPage from './pages/SeparacaoPage'
import RelatoriosPage from './pages/RelatoriosPage'

export default function App() {
  return (
    <BrowserRouter>
      <Sidebar />
      <main className="main-content">
        <Routes>
          <Route path="/"              element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard"     element={<DashboardPage />} />
          <Route path="/localizacoes"  element={<LocalizacoesPage />} />
          <Route path="/recebimento"   element={<RecebimentoPage />} />
          <Route path="/separacao"     element={<SeparacaoPage />} />
          <Route path="/relatorios"    element={<RelatoriosPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  )
}
