import { useEffect, useState } from 'react'
import { api } from '../../services/api.js'
import { useAuth } from '../../context/useAuth.js'

const metrics = [['totalBooks', 'Ouvrages'], ['totalStudents', 'Lecteurs'], ['totalEmployees', 'Employés'], ['totalLoans', 'Emprunts'], ['activeSubscriptions', 'Abonnements actifs'], ['totalRevenue', 'Revenus totaux']]

export default function Statistics() {
  const { hasPermission } = useAuth()
  const canView = hasPermission('Reports.View')
  const [stats, setStats] = useState(null)
  const [error, setError] = useState('')
  useEffect(() => {
    if (canView) api.dashboard().then(setStats).catch((requestError) => setError(requestError.message))
  }, [canView])
  if (!canView) return <section className="resource-page"><div className="panel no-view-panel"><div><strong>Consultation non autorisée</strong><p>La permission Reports.View est nécessaire pour afficher ces statistiques.</p></div></div></section>
  return <section className="resource-page"><div className="page-title-row"><div><div className="eyebrow">PILOTAGE</div><h1>Statistiques</h1><p>Les indicateurs clés de votre bibliothèque.</p></div><button className="outline-action" onClick={() => window.print()}>↧ <span>Exporter</span></button></div>{error && <div className="inline-warning">Impossible de charger les statistiques : {error}</div>}<div className="stats-grid report-grid">{metrics.map(([key, label], index) => <article className="stat-card" key={key}><div className="stat-top"><span className={`stat-icon ${['violet', 'blue', 'green', 'amber', 'rose', 'violet'][index]}`}>{['▤', '♙', '♧', '⇄', '◷', '$'][index]}</span></div><p className="stat-label">{label}</p><div className="stat-value">{stats ? (stats[key] ?? 0).toLocaleString('fr-FR') : '—'}</div><div className="stat-foot">Données actuelles</div></article>)}</div><div className="panel report-note"><span>✦</span><div><strong>Rapport en temps réel</strong><p>Les chiffres sont chargés depuis les statistiques de l’API de la bibliothèque.</p></div></div></section>
}
