import { NavLink } from 'react-router-dom'
import { LayoutDashboard, MapPin, PackagePlus, PackageMinus, BarChart2 } from 'lucide-react'

export default function Sidebar() {
  return (
    <nav className="sidebar py-3">
      <div className="px-3 mb-4">
        <h5 className="text-white fw-bold mb-0">
          <span className="text-primary">AMR</span> WMS
        </h5>
        <small style={{ fontSize: '0.7rem', opacity: 0.5 }}>Sprint 8</small>
      </div>

      <ul className="nav flex-column gap-1">
        <li className="nav-item">
          <NavLink to="/dashboard"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <LayoutDashboard size={16} /> Dashboard
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/localizacoes"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <MapPin size={16} /> Localizações
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/recebimento"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <PackagePlus size={16} /> Recebimento
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/separacao"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <PackageMinus size={16} /> Separação
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/relatorios"
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <BarChart2 size={16} /> Relatórios
          </NavLink>
        </li>
      </ul>

      <div className="mt-auto px-3 py-3" style={{ opacity: 0.35, fontSize: '0.7rem' }}>
        AMR SYSTEM v1.0
      </div>
    </nav>
  )
}
