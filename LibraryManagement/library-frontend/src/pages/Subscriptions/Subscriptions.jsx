import { useCallback, useEffect, useState } from 'react'
import { useAuth } from '../../context/useAuth.js'
import { api } from '../../services/api.js'

const money = (value) => value == null ? '—' : Number(value).toLocaleString('fr-TN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const paymentLabel = (value) => ({ Paid: 'Payé', Pending: 'En attente', Failed: 'Échoué', Refunded: 'Remboursé' }[value] || value || 'Aucun paiement')
const statusClass = (status) => ({ Actif: 'active', Expiré: 'expired', 'À venir': 'upcoming', Inactif: 'inactive' }[status] || 'inactive')

export default function Subscriptions() {
  const { hasPermission } = useAuth()
  const canView = hasPermission('Subscriptions.View')
  const [subscriptions, setSubscriptions] = useState([])
  const [loading, setLoading] = useState(canView)
  const [error, setError] = useState('')

  const reload = useCallback(async () => {
    if (!canView) return
    setLoading(true)
    setError('')
    try {
      const result = await api.subscriptions()
      setSubscriptions(Array.isArray(result) ? result : [])
    } catch (requestError) {
      setError(requestError.message || 'Impossible de charger les abonnements.')
    } finally {
      setLoading(false)
    }
  }, [canView])

  useEffect(() => { reload() }, [reload])

  const activeCount = subscriptions.filter((item) => item.status === 'Actif').length

  return <section className="resource-page subscriptions-page">
    <div className="page-title-row">
      <div><div className="eyebrow">GESTION DE LA BIBLIOTHÈQUE</div><h1>Abonnements</h1><p>Consultez les formules, les périodes et les paiements des lecteurs.</p></div>
      {canView && <button className="outline-action" onClick={reload} disabled={loading}>↻ <span>Actualiser</span></button>}
    </div>

    {!canView
      ? <div className="panel no-view-panel"><div><strong>Consultation non autorisée</strong><p>Un administrateur doit vous attribuer la permission Subscriptions.View.</p></div></div>
      : <>
        {error && <div className="inline-warning" role="alert">{error}</div>}
        <div className="subscription-summary">
          <article className="stat-card"><p className="stat-label">Abonnements enregistrés</p><div className="stat-value">{subscriptions.length}</div></article>
          <article className="stat-card"><p className="stat-label">Abonnements actifs</p><div className="stat-value">{activeCount}</div></article>
          <article className="stat-card"><p className="stat-label">Autres statuts</p><div className="stat-value">{subscriptions.length - activeCount}</div></article>
        </div>
        <div className="panel resource-table subscription-table">
          <div className="table-scroll"><table><thead><tr>
            <th>ÉTUDIANT</th><th>FORMULE</th><th>PRIX</th><th>PÉRIODE</th><th>STATUT</th><th>PAIEMENT</th>
          </tr></thead><tbody>
            {loading
              ? <tr><td className="empty-state" colSpan="6">Chargement des abonnements…</td></tr>
              : subscriptions.length
                ? subscriptions.map((item) => <tr key={item.id}>
                  <td><strong>{item.studentName}</strong><small className="subscription-email">{item.studentEmail}</small></td>
                  <td><strong>{item.planName}</strong><small className="subscription-email">{item.durationDays} jours · {item.maxLoans} emprunts maximum</small></td>
                  <td>{money(item.price)}</td>
                  <td>{new Intl.DateTimeFormat('fr-FR').format(new Date(item.startDate))} – {new Intl.DateTimeFormat('fr-FR').format(new Date(item.endDate))}</td>
                  <td><span className={`subscription-status ${statusClass(item.status)}`}>{item.status}</span></td>
                  <td><strong>{paymentLabel(item.paymentStatus)}</strong><small className="subscription-email">{item.paymentAmount == null ? '—' : money(item.paymentAmount)}{item.paymentMethod ? ` · ${item.paymentMethod}` : ''}</small></td>
                </tr>)
                : <tr><td className="empty-state" colSpan="6">Aucun abonnement à afficher pour le moment.</td></tr>}
          </tbody></table></div>
          <div className="table-bottom"><span>{subscriptions.length} abonnement{subscriptions.length === 1 ? '' : 's'}</span></div>
        </div>
      </>}
  </section>
}
