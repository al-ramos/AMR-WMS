export default function DashboardPage() {
  return (
    <div>
      <h4 className="mb-1">Dashboard</h4>
      <p className="text-muted mb-4">Visão geral do armazém</p>

      <div className="card border-0 shadow-sm">
        <div className="card-body text-center py-5">
          <div className="mb-3" style={{ fontSize: '3rem' }}>🏗️</div>
          <h5 className="text-muted">WMS em construção</h5>
          <p className="text-muted small mb-0">
            Sprint 8 — Módulo de Warehouse Management System
          </p>
        </div>
      </div>
    </div>
  )
}
