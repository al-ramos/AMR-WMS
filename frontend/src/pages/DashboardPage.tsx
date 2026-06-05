import { useState, useEffect, useCallback } from 'react'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Cell, ResponsiveContainer,
} from 'recharts'
import { AlertTriangle, Warehouse, BarChart2, PackageCheck, Truck } from 'lucide-react'

// ── Types ──────────────────────────────────────────────────────────────────────

interface DashboardWmsKpisDto {
  totalLocalizacoes: number
  ocupacaoMediaPct:  number
  localizacoesAlerta: number
  ordemsSeparacao:   number
  ordensRecebimento: number
}

interface OcupacaoPorZonaDto {
  zona:             string
  totalLocalizacoes: number
  capacidadeTotal:  number
  ocupacaoTotal:    number
  ocupacaoPct:      number
}

interface TopProdutoDto {
  produtoId:          number
  totalMovimentacoes: number
  totalQuantidade:    number
}

interface AlertaLocalizacaoDto {
  id:          string
  codigo:      string
  zona:        string
  ocupacao:    number
  capacidade:  number
  ocupacaoPct: number
}

// ── Helpers ────────────────────────────────────────────────────────────────────

async function apiFetch<T>(url: string): Promise<T> {
  const res = await fetch(url)
  if (!res.ok) throw new Error(`Erro ${res.status}`)
  return res.json()
}

function ocupacaoColor(pct: number): string {
  if (pct < 70) return '#198754'
  if (pct < 90) return '#ffc107'
  return '#dc3545'
}

const PERIODO_LABELS: Record<number, string> = { 1: 'Hoje', 7: 'Últimos 7 dias', 30: 'Últimos 30 dias' }

// ── KPI Card ──────────────────────────────────────────────────────────────────

function KpiCard({
  label, value, sub, icon: Icon, color,
}: {
  label: string; value: string | number; sub?: string
  icon: React.ElementType; color: string
}) {
  return (
    <div className="col-sm-6 col-xl-3">
      <div className="card border-0 shadow-sm h-100">
        <div className="card-body d-flex align-items-center gap-3">
          <div
            className="rounded-3 d-flex align-items-center justify-content-center flex-shrink-0"
            style={{ width: 52, height: 52, backgroundColor: `${color}20` }}
          >
            <Icon size={24} color={color} />
          </div>
          <div>
            <div className="text-muted small">{label}</div>
            <div className="fw-bold fs-4 lh-1">{value}</div>
            {sub && <div className="text-muted" style={{ fontSize: '0.72rem' }}>{sub}</div>}
          </div>
        </div>
      </div>
    </div>
  )
}

// ── Main Component ─────────────────────────────────────────────────────────────

export default function DashboardPage() {
  const [dias, setDias] = useState(1)
  const [kpis, setKpis] = useState<DashboardWmsKpisDto | null>(null)
  const [zonas, setZonas] = useState<OcupacaoPorZonaDto[]>([])
  const [topProdutos, setTopProdutos] = useState<TopProdutoDto[]>([])
  const [alertas, setAlertas] = useState<AlertaLocalizacaoDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchAll = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const [k, z, t, a] = await Promise.all([
        apiFetch<DashboardWmsKpisDto>(`/api/dashboard/wms?dias=${dias}`),
        apiFetch<OcupacaoPorZonaDto[]>('/api/dashboard/wms/ocupacao'),
        apiFetch<TopProdutoDto[]>(`/api/dashboard/wms/top-produtos?dias=${dias}`),
        apiFetch<AlertaLocalizacaoDto[]>('/api/dashboard/wms/alertas'),
      ])
      setKpis(k); setZonas(z); setTopProdutos(t); setAlertas(a)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar dashboard')
    } finally {
      setLoading(false)
    }
  }, [dias])

  useEffect(() => { fetchAll() }, [fetchAll])

  return (
    <div>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h4 className="mb-1">Dashboard WMS</h4>
          <p className="text-muted mb-0">Visão geral do armazém</p>
        </div>
        <div className="btn-group" role="group">
          {([1, 7, 30] as const).map(d => (
            <button
              key={d}
              className={`btn btn-sm ${dias === d ? 'btn-primary' : 'btn-outline-primary'}`}
              onClick={() => setDias(d)}
            >
              {PERIODO_LABELS[d]}
            </button>
          ))}
        </div>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      {loading && (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
        </div>
      )}

      {!loading && kpis && (
        <>
          {/* KPI Cards */}
          <div className="row g-3 mb-4">
            <KpiCard
              label="Localizações"
              value={kpis.totalLocalizacoes}
              sub="total cadastradas"
              icon={Warehouse}
              color="#0d6efd"
            />
            <KpiCard
              label="Ocupação Média"
              value={`${kpis.ocupacaoMediaPct}%`}
              sub={`${kpis.localizacoesAlerta} alerta(s) >90%`}
              icon={BarChart2}
              color={ocupacaoColor(kpis.ocupacaoMediaPct)}
            />
            <KpiCard
              label="Ordens Separação"
              value={kpis.ordemsSeparacao}
              sub={PERIODO_LABELS[dias].toLowerCase()}
              icon={PackageCheck}
              color="#198754"
            />
            <KpiCard
              label="Ordens Recebimento"
              value={kpis.ordensRecebimento}
              sub={`${PERIODO_LABELS[dias].toLowerCase()} (Core)`}
              icon={Truck}
              color="#6f42c1"
            />
          </div>

          <div className="row g-3 mb-4">
            {/* Gráfico Ocupação por Zona */}
            <div className="col-lg-7">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-header bg-white border-0 pt-3 pb-0">
                  <h6 className="fw-semibold mb-0">Ocupação por Zona</h6>
                </div>
                <div className="card-body">
                  {zonas.length === 0 ? (
                    <div className="text-center text-muted py-4 small">Sem dados de zona.</div>
                  ) : (
                    <ResponsiveContainer width="100%" height={220}>
                      <BarChart data={zonas} margin={{ top: 8, right: 16, left: 0, bottom: 0 }}>
                        <CartesianGrid strokeDasharray="3 3" vertical={false} />
                        <XAxis dataKey="zona" tick={{ fontSize: 12 }} />
                        <YAxis domain={[0, 100]} tickFormatter={v => `${v}%`} tick={{ fontSize: 11 }} />
                        <Tooltip
                          formatter={(val: number) => [`${val}%`, 'Ocupação']}
                          labelFormatter={l => `Zona ${l}`}
                        />
                        <Bar dataKey="ocupacaoPct" radius={[4, 4, 0, 0]} name="Ocupação">
                          {zonas.map((z, i) => (
                            <Cell key={i} fill={ocupacaoColor(z.ocupacaoPct)} />
                          ))}
                        </Bar>
                      </BarChart>
                    </ResponsiveContainer>
                  )}
                  {zonas.length > 0 && (
                    <div className="d-flex gap-3 mt-2 justify-content-center small text-muted">
                      <span><span style={{ color: '#198754' }}>■</span> {'<'}70% livre</span>
                      <span><span style={{ color: '#ffc107' }}>■</span> 70–90% atenção</span>
                      <span><span style={{ color: '#dc3545' }}>■</span> {'>'}90% crítico</span>
                    </div>
                  )}
                </div>
              </div>
            </div>

            {/* Alertas */}
            <div className="col-lg-5">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-header bg-white border-0 pt-3 pb-0 d-flex align-items-center gap-2">
                  <AlertTriangle size={16} color="#dc3545" />
                  <h6 className="fw-semibold mb-0">Alertas — Ocupação {'>'}90%</h6>
                  {alertas.length > 0 && (
                    <span className="badge bg-danger ms-auto">{alertas.length}</span>
                  )}
                </div>
                <div className="card-body p-0">
                  {alertas.length === 0 ? (
                    <div className="text-center text-muted py-4 small px-3">
                      Nenhuma localização em situação crítica.
                    </div>
                  ) : (
                    <div style={{ maxHeight: 240, overflowY: 'auto' }}>
                      <table className="table table-sm mb-0">
                        <thead className="table-light sticky-top">
                          <tr>
                            <th>Código</th>
                            <th>Zona</th>
                            <th className="text-end">Ocupação</th>
                          </tr>
                        </thead>
                        <tbody>
                          {alertas.map(a => (
                            <tr key={a.id}>
                              <td className="font-monospace small">{a.codigo}</td>
                              <td><span className="badge bg-secondary">{a.zona}</span></td>
                              <td className="text-end">
                                <span className="badge bg-danger">{a.ocupacaoPct}%</span>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Top Produtos */}
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white border-0 pt-3 pb-0">
              <h6 className="fw-semibold mb-0">
                Top 10 Produtos Movimentados
                <span className="text-muted fw-normal ms-2 small">— {PERIODO_LABELS[dias].toLowerCase()}</span>
              </h6>
            </div>
            <div className="card-body p-0">
              {topProdutos.length === 0 ? (
                <div className="text-center text-muted py-4 small">
                  Nenhuma movimentação registrada no período.
                </div>
              ) : (
                <table className="table table-sm table-hover mb-0">
                  <thead className="table-light">
                    <tr>
                      <th>#</th>
                      <th>Produto</th>
                      <th className="text-end">Movimentações</th>
                      <th className="text-end">Quantidade Total</th>
                    </tr>
                  </thead>
                  <tbody>
                    {topProdutos.map((p, i) => (
                      <tr key={p.produtoId}>
                        <td className="text-muted small">{i + 1}</td>
                        <td className="fw-semibold">Produto {p.produtoId}</td>
                        <td className="text-end">{p.totalMovimentacoes}</td>
                        <td className="text-end text-muted">{p.totalQuantidade}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  )
}
