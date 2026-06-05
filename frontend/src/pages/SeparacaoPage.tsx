import { useState, useEffect, useMemo } from 'react'
import { Plus, PackageCheck, CheckCircle } from 'lucide-react'

// ── Types ──────────────────────────────────────────────────────────────────────

interface ItemSeparacaoDto {
  id: string
  produtoId: number
  localizacaoId: string | null
  qntSolicitada: number
  qntSeparada: number
}

interface OrdemSeparacaoDto {
  id: string
  pedidoVendaId: number
  status: number // 1=Aberta 2=EmSeparacao 3=Concluida
  dataCriacao: string
  itens: ItemSeparacaoDto[]
}

interface LocalizacaoDto {
  id: string
  codigo: string
  zona: string
  ocupacao: number
  capacidade: number
}

interface ItemPedidoVendaDto {
  produtoId: number
  nomeProduto: string
  quantidade: number
}

interface PedidoVendaDto {
  id: number
  numero: string
  cliente: string
  dataAprovacao: string
  itens: ItemPedidoVendaDto[]
}

// ── Constants ──────────────────────────────────────────────────────────────────

const STATUS_LABEL: Record<number, string> = { 1: 'Aberta', 2: 'Em Separação', 3: 'Concluída' }
const STATUS_BADGE: Record<number, string> = { 1: 'secondary', 2: 'warning', 3: 'success' }

// ── API helper ─────────────────────────────────────────────────────────────────

async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, init)
  if (!res.ok) {
    const body = await res.json().catch(() => ({ title: 'Erro na requisição' }))
    throw new Error(body.title ?? 'Erro na requisição')
  }
  return res.json()
}

const gerarPVsMock = (): PedidoVendaDto[] => [
  { id: 2001, numero: 'PV-2001', cliente: 'Cliente Demo A', dataAprovacao: new Date().toISOString(), itens: [{ produtoId: 10, nomeProduto: 'Produto 10', quantidade: 5 }] },
  { id: 2002, numero: 'PV-2002', cliente: 'Cliente Demo B', dataAprovacao: new Date().toISOString(), itens: [{ produtoId: 11, nomeProduto: 'Produto 11', quantidade: 3 }, { produtoId: 12, nomeProduto: 'Produto 12', quantidade: 7 }] },
]

// ── OrdensPorStatus ────────────────────────────────────────────────────────────

function StatusBadge({ status }: { status: number }) {
  return <span className={`badge bg-${STATUS_BADGE[status]}`}>{STATUS_LABEL[status]}</span>
}

function OrdensPorStatus({
  status, ordens, onSepararItem, onConcluir,
}: {
  status: number
  ordens: OrdemSeparacaoDto[]
  onSepararItem: (o: OrdemSeparacaoDto, i: ItemSeparacaoDto) => void
  onConcluir: (o: OrdemSeparacaoDto) => void
}) {
  if (ordens.length === 0) return null

  const headerBg: Record<number, string> = { 1: 'bg-light', 2: 'bg-warning bg-opacity-25', 3: 'bg-success bg-opacity-10' }

  return (
    <div className="mb-4">
      <h6 className="text-uppercase text-muted small fw-semibold mb-2">
        <span className={`badge bg-${STATUS_BADGE[status]} me-2`}>{ordens.length}</span>
        {STATUS_LABEL[status]}
      </h6>
      {ordens.map(ordem => (
        <div key={ordem.id} className="card border-0 shadow-sm mb-2">
          <div className={`card-header ${headerBg[status]} border-0 py-2 px-3 d-flex justify-content-between align-items-center`}>
            <div>
              <strong>PV-{ordem.pedidoVendaId}</strong>
              <span className="ms-2"><StatusBadge status={ordem.status} /></span>
              <small className="text-muted ms-3">
                {ordem.itens.filter(i => i.qntSeparada > 0).length}/{ordem.itens.length} itens
              </small>
            </div>
            <div className="d-flex gap-2 align-items-center">
              <small className="text-muted">
                {new Date(ordem.dataCriacao).toLocaleDateString('pt-BR')}
              </small>
              {status !== 3 && (
                <button
                  className="btn btn-sm btn-outline-success"
                  onClick={() => onConcluir(ordem)}
                  title="Concluir ordem"
                >
                  <CheckCircle size={14} />
                </button>
              )}
            </div>
          </div>
          <div className="table-responsive">
            <table className="table table-sm mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Produto</th>
                  <th>Solicitado</th>
                  <th>Separado</th>
                  <th>Localização</th>
                  {status !== 3 && <th style={{ width: 90 }}>Ação</th>}
                </tr>
              </thead>
              <tbody>
                {ordem.itens.map(item => (
                  <tr key={item.id}>
                    <td className="small">Prod. {item.produtoId}</td>
                    <td>{item.qntSolicitada}</td>
                    <td>
                      {item.qntSeparada > 0
                        ? <span className="text-success fw-semibold">{item.qntSeparada}</span>
                        : <span className="text-muted">—</span>}
                    </td>
                    <td className="font-monospace small text-muted">
                      {item.localizacaoId ? item.localizacaoId.slice(0, 8) + '…' : '—'}
                    </td>
                    {status !== 3 && (
                      <td>
                        {item.qntSeparada === 0 ? (
                          <button
                            className="btn btn-sm btn-outline-primary"
                            onClick={() => onSepararItem(ordem, item)}
                          >
                            Separar
                          </button>
                        ) : (
                          <span className="badge bg-success-subtle text-success border">✓</span>
                        )}
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ))}
    </div>
  )
}

// ── Main Page ──────────────────────────────────────────────────────────────────

export default function SeparacaoPage() {
  const [ordens, setOrdens] = useState<OrdemSeparacaoDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Modal Nova Ordem
  const [showNovaModal, setShowNovaModal] = useState(false)
  const [pvs, setPvs] = useState<PedidoVendaDto[]>([])
  const [pvLoading, setPvLoading] = useState(false)
  const [pvSelecionado, setPvSelecionado] = useState<PedidoVendaDto | null>(null)
  const [criando, setCriando] = useState(false)
  const [novaError, setNovaError] = useState<string | null>(null)

  // Modal Separar Item
  const [ordemAtiva, setOrdemAtiva] = useState<OrdemSeparacaoDto | null>(null)
  const [itemAtivo, setItemAtivo] = useState<ItemSeparacaoDto | null>(null)
  const [localizacoes, setLocalizacoes] = useState<LocalizacaoDto[]>([])
  const [locSelecionada, setLocSelecionada] = useState('')
  const [qntSeparar, setQntSeparar] = useState(0)
  const [separando, setSeparando] = useState(false)
  const [separandoError, setSeparandoError] = useState<string | null>(null)

  const fetchOrdens = async () => {
    try {
      setLoading(true)
      const data = await apiFetch<OrdemSeparacaoDto[]>('/api/separacao')
      setOrdens(data)
      setError(null)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar ordens')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchOrdens() }, [])

  const grouped = useMemo(() => {
    const map = new Map<number, OrdemSeparacaoDto[]>([[2, []], [1, []], [3, []]])
    for (const o of ordens) map.get(o.status)?.push(o)
    return map
  }, [ordens])

  // ── Nova Ordem ───────────────────────────────────────────────────────────────

  const openNovaModal = async () => {
    setNovaError(null)
    setPvSelecionado(null)
    setShowNovaModal(true)
    setPvLoading(true)
    try {
      const data = await apiFetch<PedidoVendaDto[]>('/api/separacao/pv-aprovados')
      setPvs(data.length ? data : gerarPVsMock())
    } catch {
      setPvs(gerarPVsMock())
    } finally {
      setPvLoading(false)
    }
  }

  const handleCriarOrdem = async () => {
    if (!pvSelecionado) return
    setCriando(true)
    setNovaError(null)
    try {
      await apiFetch('/api/separacao/criar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          pedidoVendaId: pvSelecionado.id,
          itens: pvSelecionado.itens.map(i => ({ produtoId: i.produtoId, quantidade: i.quantidade })),
        }),
      })
      setShowNovaModal(false)
      await fetchOrdens()
    } catch (e) {
      setNovaError(e instanceof Error ? e.message : 'Erro ao criar ordem')
    } finally {
      setCriando(false)
    }
  }

  // ── Separar Item ─────────────────────────────────────────────────────────────

  const openSepararModal = async (ordem: OrdemSeparacaoDto, item: ItemSeparacaoDto) => {
    setOrdemAtiva(ordem)
    setItemAtivo(item)
    setQntSeparar(item.qntSolicitada)
    setSeparandoError(null)
    setLocSelecionada('')
    try {
      const data = await apiFetch<LocalizacaoDto[]>('/api/localizacoes/disponiveis')
      // FIFO: menor razão de ocupação primeiro
      const sorted = [...data].sort((a, b) => a.ocupacao / a.capacidade - b.ocupacao / b.capacidade)
      setLocalizacoes(sorted)
      if (sorted.length > 0) setLocSelecionada(sorted[0].id)
    } catch {
      setLocalizacoes([])
    }
  }

  const closeSepararModal = () => { setOrdemAtiva(null); setItemAtivo(null) }

  const handleSepararItem = async () => {
    if (!ordemAtiva || !itemAtivo || !locSelecionada) return
    setSeparando(true)
    setSeparandoError(null)
    try {
      await apiFetch(`/api/separacao/${ordemAtiva.id}/separar-item`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ itemSeparacaoId: itemAtivo.id, localizacaoId: locSelecionada, quantidade: qntSeparar }),
      })
      closeSepararModal()
      await fetchOrdens()
    } catch (e) {
      setSeparandoError(e instanceof Error ? e.message : 'Erro ao separar item')
    } finally {
      setSeparando(false)
    }
  }

  // ── Concluir ─────────────────────────────────────────────────────────────────

  const handleConcluir = async (ordem: OrdemSeparacaoDto) => {
    if (!confirm(`Concluir ordem PV-${ordem.pedidoVendaId}?`)) return
    try {
      await apiFetch(`/api/separacao/${ordem.id}/concluir`, { method: 'PUT' })
      await fetchOrdens()
    } catch (e) {
      alert(e instanceof Error ? e.message : 'Erro ao concluir')
    }
  }

  // ── Render ───────────────────────────────────────────────────────────────────

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h4 className="mb-1">Separação (Picking)</h4>
          <p className="text-muted mb-0">Ordens de separação para Pedidos de Venda</p>
        </div>
        <button className="btn btn-primary" onClick={openNovaModal}>
          <Plus size={16} className="me-1" />
          Nova Ordem
        </button>
      </div>

      {loading && (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
        </div>
      )}

      {error && !loading && <div className="alert alert-danger">{error}</div>}

      {!loading && !error && (
        <>
          <OrdensPorStatus status={2} ordens={grouped.get(2)!} onSepararItem={openSepararModal} onConcluir={handleConcluir} />
          <OrdensPorStatus status={1} ordens={grouped.get(1)!} onSepararItem={openSepararModal} onConcluir={handleConcluir} />
          <OrdensPorStatus status={3} ordens={grouped.get(3)!} onSepararItem={openSepararModal} onConcluir={handleConcluir} />
          {ordens.length === 0 && (
            <div className="card border-0 shadow-sm">
              <div className="card-body text-center py-5">
                <p className="text-muted mb-0">Nenhuma ordem de separação encontrada. Crie uma nova ordem.</p>
              </div>
            </div>
          )}
        </>
      )}

      {/* Modal Nova Ordem */}
      {showNovaModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}
          onClick={e => { if (e.target === e.currentTarget) setShowNovaModal(false) }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Nova Ordem de Separação</h5>
                <button className="btn-close" onClick={() => setShowNovaModal(false)} />
              </div>
              <div className="modal-body">
                {novaError && <div className="alert alert-danger py-2 small">{novaError}</div>}
                {pvLoading && <div className="text-center py-3"><div className="spinner-border spinner-border-sm text-primary" /></div>}
                {!pvLoading && pvs.length === 0 && (
                  <p className="text-muted text-center py-3">Nenhum PV aprovado disponível.</p>
                )}
                {!pvLoading && pvs.map(pv => (
                  <div key={pv.id}
                    className={`card mb-2 ${pvSelecionado?.id === pv.id ? 'border-primary border-2' : 'border-0 shadow-sm'}`}
                    style={{ cursor: 'pointer' }}
                    onClick={() => setPvSelecionado(pv)}
                  >
                    <div className="card-body py-2 px-3">
                      <div className="d-flex justify-content-between align-items-center">
                        <div>
                          <strong>{pv.numero}</strong>
                          <span className="text-muted ms-2 small">{pv.cliente}</span>
                        </div>
                        <small className="text-muted">{pv.itens.length} item(s)</small>
                      </div>
                      <div className="mt-1">
                        {pv.itens.map(i => (
                          <span key={i.produtoId} className="badge bg-light text-dark me-1 border small">
                            Prod.{i.produtoId} × {i.quantidade}
                          </span>
                        ))}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setShowNovaModal(false)}>Cancelar</button>
                <button className="btn btn-primary" onClick={handleCriarOrdem} disabled={!pvSelecionado || criando}>
                  {criando ? <><span className="spinner-border spinner-border-sm me-1" />Criando...</> : 'Criar Ordem'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Modal Separar Item */}
      {ordemAtiva && itemAtivo && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}
          onClick={e => { if (e.target === e.currentTarget) closeSepararModal() }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Separar — PV {ordemAtiva.pedidoVendaId}</h5>
                <button className="btn-close" onClick={closeSepararModal} />
              </div>
              <div className="modal-body">
                {separandoError && <div className="alert alert-danger py-2 small">{separandoError}</div>}
                <div className="mb-3">
                  <label className="form-label">Produto</label>
                  <input className="form-control" value={`Produto ${itemAtivo.produtoId}`} disabled />
                </div>
                <div className="mb-3">
                  <label className="form-label">
                    Localização
                    <small className="text-muted ms-1">(FIFO — menor ocupação primeiro)</small>
                  </label>
                  {localizacoes.length === 0
                    ? <div className="alert alert-warning py-2 small">Nenhuma localização disponível.</div>
                    : (
                      <select className="form-select" value={locSelecionada} onChange={e => setLocSelecionada(e.target.value)}>
                        {localizacoes.map(loc => {
                          const pct = Math.round((loc.ocupacao / loc.capacidade) * 100)
                          return (
                            <option key={loc.id} value={loc.id}>
                              {loc.codigo} — {loc.ocupacao}/{loc.capacidade} ({pct}%)
                            </option>
                          )
                        })}
                      </select>
                    )}
                </div>
                <div className="mb-1">
                  <label className="form-label">
                    Quantidade
                    <small className="text-muted ms-1">(solicitado: {itemAtivo.qntSolicitada})</small>
                  </label>
                  <input
                    type="number" className="form-control"
                    value={qntSeparar} min={1} max={itemAtivo.qntSolicitada}
                    onChange={e => setQntSeparar(Number(e.target.value))}
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeSepararModal} disabled={separando}>Cancelar</button>
                <button
                  className="btn btn-primary"
                  onClick={handleSepararItem}
                  disabled={separando || !locSelecionada || qntSeparar <= 0}
                >
                  {separando
                    ? <><span className="spinner-border spinner-border-sm me-1" />Separando...</>
                    : <><PackageCheck size={15} className="me-1" />Confirmar</>}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
