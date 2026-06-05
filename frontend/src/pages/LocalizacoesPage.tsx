import { useState, useEffect, useMemo } from 'react'
import { Plus, Edit2, Trash2 } from 'lucide-react'

interface LocalizacaoDto {
  id: string
  codigo: string
  zona: string
  corredor: string
  prateleira: string
  posicao: string
  capacidade: number
  ocupacao: number
  tipoLocalizacao: number
  criadoEm: string
}

interface FormState {
  zona: string
  corredor: string
  prateleira: string
  posicao: string
  capacidade: number
  tipoLocalizacao: number
}

const TIPO_LABELS: Record<number, string> = {
  1: 'Picking',
  2: 'Reserva',
  3: 'Expedição',
  4: 'Bloqueado',
}

const TIPO_BADGE: Record<number, string> = {
  1: 'primary',
  2: 'secondary',
  3: 'success',
  4: 'danger',
}

const EMPTY_FORM: FormState = {
  zona: '',
  corredor: '',
  prateleira: '',
  posicao: '',
  capacidade: 50,
  tipoLocalizacao: 1,
}

function OcupacaoBar({ ocupacao, capacidade }: { ocupacao: number; capacidade: number }) {
  const pct = capacidade > 0 ? Math.round((ocupacao / capacidade) * 100) : 0
  const color = pct < 70 ? 'success' : pct < 90 ? 'warning' : 'danger'
  return (
    <div style={{ minWidth: 140 }}>
      <div className="progress mb-1" style={{ height: 8 }}>
        <div className={`progress-bar bg-${color}`} style={{ width: `${pct}%` }} />
      </div>
      <small className="text-muted">
        {ocupacao}/{capacidade} ({pct}%)
      </small>
    </div>
  )
}

export default function LocalizacoesPage() {
  const [localizacoes, setLocalizacoes] = useState<LocalizacaoDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [filtroZona, setFiltroZona] = useState('')
  const [filtroTipo, setFiltroTipo] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [editando, setEditando] = useState<LocalizacaoDto | null>(null)
  const [form, setForm] = useState<FormState>(EMPTY_FORM)
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)

  const fetchLocalizacoes = async () => {
    try {
      setLoading(true)
      const res = await fetch('/api/localizacoes')
      if (!res.ok) throw new Error('Falha ao carregar localizações')
      const data: LocalizacaoDto[] = await res.json()
      setLocalizacoes(data)
      setError(null)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro desconhecido')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchLocalizacoes() }, [])

  const zonas = useMemo(
    () => [...new Set(localizacoes.map(l => l.zona))].sort(),
    [localizacoes]
  )

  const filtered = useMemo(
    () =>
      localizacoes.filter(l => {
        if (filtroZona && l.zona !== filtroZona) return false
        if (filtroTipo && l.tipoLocalizacao !== Number(filtroTipo)) return false
        return true
      }),
    [localizacoes, filtroZona, filtroTipo]
  )

  const grouped = useMemo(() => {
    const map = new Map<string, LocalizacaoDto[]>()
    for (const l of filtered) {
      if (!map.has(l.zona)) map.set(l.zona, [])
      map.get(l.zona)!.push(l)
    }
    return [...map.entries()].sort(([a], [b]) => a.localeCompare(b))
  }, [filtered])

  const openCriar = () => {
    setEditando(null)
    setForm(EMPTY_FORM)
    setFormError(null)
    setShowModal(true)
  }

  const openEditar = (loc: LocalizacaoDto) => {
    setEditando(loc)
    setForm({
      zona: loc.zona,
      corredor: loc.corredor,
      prateleira: loc.prateleira,
      posicao: loc.posicao,
      capacidade: loc.capacidade,
      tipoLocalizacao: loc.tipoLocalizacao,
    })
    setFormError(null)
    setShowModal(true)
  }

  const closeModal = () => setShowModal(false)

  const handleSave = async () => {
    setSaving(true)
    setFormError(null)
    try {
      let res: Response
      if (editando) {
        res = await fetch(`/api/localizacoes/${editando.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            capacidade: form.capacidade,
            tipoLocalizacao: form.tipoLocalizacao,
          }),
        })
      } else {
        res = await fetch('/api/localizacoes', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(form),
        })
      }
      if (!res.ok) {
        const body = await res.json().catch(() => ({ title: 'Erro ao salvar' }))
        throw new Error(body.title ?? 'Erro ao salvar')
      }
      closeModal()
      await fetchLocalizacoes()
    } catch (e) {
      setFormError(e instanceof Error ? e.message : 'Erro desconhecido')
    } finally {
      setSaving(false)
    }
  }

  const handleRemover = async (loc: LocalizacaoDto) => {
    if (!confirm(`Remover localização ${loc.codigo}?`)) return
    try {
      const res = await fetch(`/api/localizacoes/${loc.id}`, { method: 'DELETE' })
      if (!res.ok) {
        const body = await res.json().catch(() => ({ title: 'Erro ao remover' }))
        alert(body.title ?? 'Erro ao remover')
        return
      }
      await fetchLocalizacoes()
    } catch {
      alert('Erro de rede ao remover localização')
    }
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h4 className="mb-1">Localizações</h4>
          <p className="text-muted mb-0">Gestão de endereços do armazém</p>
        </div>
        <button className="btn btn-primary" onClick={openCriar}>
          <Plus size={16} className="me-1" />
          Nova Localização
        </button>
      </div>

      {/* Filtros */}
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body py-2">
          <div className="row g-2 align-items-center">
            <div className="col-auto">
              <label className="form-label mb-0 small fw-semibold">Zona</label>
            </div>
            <div className="col-auto">
              <select
                className="form-select form-select-sm"
                value={filtroZona}
                onChange={e => setFiltroZona(e.target.value)}
              >
                <option value="">Todas</option>
                {zonas.map(z => (
                  <option key={z} value={z}>{z}</option>
                ))}
              </select>
            </div>
            <div className="col-auto ms-2">
              <label className="form-label mb-0 small fw-semibold">Tipo</label>
            </div>
            <div className="col-auto">
              <select
                className="form-select form-select-sm"
                value={filtroTipo}
                onChange={e => setFiltroTipo(e.target.value)}
              >
                <option value="">Todos</option>
                {Object.entries(TIPO_LABELS).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            {(filtroZona || filtroTipo) && (
              <div className="col-auto ms-1">
                <button
                  className="btn btn-link btn-sm text-secondary p-0"
                  onClick={() => { setFiltroZona(''); setFiltroTipo('') }}
                >
                  Limpar
                </button>
              </div>
            )}
            <div className="col-auto ms-auto">
              <small className="text-muted">{filtered.length} localização(ões)</small>
            </div>
          </div>
        </div>
      </div>

      {/* Legenda de ocupação */}
      <div className="d-flex gap-3 mb-3 small">
        <span><span className="badge bg-success me-1">&nbsp;</span>{'<'}70% livre</span>
        <span><span className="badge bg-warning me-1">&nbsp;</span>70–90% ocupado</span>
        <span><span className="badge bg-danger me-1">&nbsp;</span>{'>'}90% crítico</span>
      </div>

      {loading && (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Carregando...</span>
          </div>
        </div>
      )}

      {error && !loading && (
        <div className="alert alert-danger">{error}</div>
      )}

      {!loading && !error && grouped.length === 0 && (
        <div className="card border-0 shadow-sm">
          <div className="card-body text-center py-5">
            <p className="text-muted mb-0">Nenhuma localização encontrada.</p>
          </div>
        </div>
      )}

      {!loading && !error && grouped.map(([zona, locs]) => (
        <div key={zona} className="card border-0 shadow-sm mb-3">
          <div className="card-header bg-light border-0 py-2 px-3">
            <strong>Zona {zona}</strong>
            <span className="badge bg-secondary ms-2">{locs.length}</span>
          </div>
          <div className="table-responsive">
            <table className="table table-sm table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Código</th>
                  <th>Tipo</th>
                  <th>Ocupação</th>
                  <th style={{ width: 90 }}>Ações</th>
                </tr>
              </thead>
              <tbody>
                {locs.map(loc => (
                  <tr key={loc.id}>
                    <td className="fw-semibold font-monospace small">{loc.codigo}</td>
                    <td>
                      <span className={`badge bg-${TIPO_BADGE[loc.tipoLocalizacao]}`}>
                        {TIPO_LABELS[loc.tipoLocalizacao]}
                      </span>
                    </td>
                    <td>
                      <OcupacaoBar ocupacao={loc.ocupacao} capacidade={loc.capacidade} />
                    </td>
                    <td>
                      <button
                        className="btn btn-sm btn-outline-secondary me-1"
                        title="Editar"
                        onClick={() => openEditar(loc)}
                      >
                        <Edit2 size={14} />
                      </button>
                      <button
                        className="btn btn-sm btn-outline-danger"
                        title={loc.ocupacao > 0 ? 'Não é possível remover com estoque' : 'Remover'}
                        onClick={() => handleRemover(loc)}
                        disabled={loc.ocupacao > 0}
                      >
                        <Trash2 size={14} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ))}

      {/* Modal Criar/Editar */}
      {showModal && (
        <div
          className="modal show d-block"
          style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}
          onClick={e => { if (e.target === e.currentTarget) closeModal() }}
        >
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  {editando ? `Editar — ${editando.codigo}` : 'Nova Localização'}
                </h5>
                <button type="button" className="btn-close" onClick={closeModal} />
              </div>
              <div className="modal-body">
                {formError && (
                  <div className="alert alert-danger py-2 small">{formError}</div>
                )}

                {!editando && (
                  <>
                    <div className="row g-3 mb-3">
                      <div className="col-6">
                        <label className="form-label">Zona *</label>
                        <input
                          className="form-control"
                          value={form.zona}
                          onChange={e => setForm(f => ({ ...f, zona: e.target.value }))}
                          maxLength={10}
                          placeholder="Ex: A"
                          autoFocus
                        />
                      </div>
                      <div className="col-6">
                        <label className="form-label">Corredor *</label>
                        <input
                          className="form-control"
                          value={form.corredor}
                          onChange={e => setForm(f => ({ ...f, corredor: e.target.value }))}
                          maxLength={10}
                          placeholder="Ex: 01"
                        />
                      </div>
                    </div>
                    <div className="row g-3 mb-3">
                      <div className="col-6">
                        <label className="form-label">Prateleira *</label>
                        <input
                          className="form-control"
                          value={form.prateleira}
                          onChange={e => setForm(f => ({ ...f, prateleira: e.target.value }))}
                          maxLength={10}
                          placeholder="Ex: P1"
                        />
                      </div>
                      <div className="col-6">
                        <label className="form-label">Posição *</label>
                        <input
                          className="form-control"
                          value={form.posicao}
                          onChange={e => setForm(f => ({ ...f, posicao: e.target.value }))}
                          maxLength={10}
                          placeholder="Ex: 001"
                        />
                      </div>
                    </div>
                  </>
                )}

                <div className="row g-3">
                  <div className="col-6">
                    <label className="form-label">Capacidade *</label>
                    <input
                      type="number"
                      className="form-control"
                      value={form.capacidade}
                      min={editando ? editando.ocupacao || 1 : 1}
                      onChange={e => setForm(f => ({ ...f, capacidade: Number(e.target.value) }))}
                    />
                    {editando && editando.ocupacao > 0 && (
                      <small className="text-muted">Ocupação atual: {editando.ocupacao}</small>
                    )}
                  </div>
                  <div className="col-6">
                    <label className="form-label">Tipo *</label>
                    <select
                      className="form-select"
                      value={form.tipoLocalizacao}
                      onChange={e => setForm(f => ({ ...f, tipoLocalizacao: Number(e.target.value) }))}
                    >
                      {Object.entries(TIPO_LABELS).map(([k, v]) => (
                        <option key={k} value={Number(k)}>{v}</option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeModal} disabled={saving}>
                  Cancelar
                </button>
                <button className="btn btn-primary" onClick={handleSave} disabled={saving}>
                  {saving ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1" role="status" />
                      Salvando...
                    </>
                  ) : 'Salvar'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
