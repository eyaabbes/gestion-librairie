import { useEffect, useState } from 'react'
import { useAuth } from '../context/useAuth.js'
import { api } from '../services/api.js'

const cards = [
  { key: 'totalBooks', label: 'Ouvrages au catalogue', icon: '▤', tone: 'violet', change: 'Collection complète' },
  { key: 'totalStudents', label: 'Lecteurs inscrits', icon: '♙', tone: 'blue', change: 'Membres enregistrés' },
  { key: 'activeLoans', label: 'Emprunts en cours', icon: '⇄', tone: 'amber', change: 'À suivre aujourd’hui' },
  { key: 'overdueLoans', label: 'Retards à traiter', icon: '◷', tone: 'rose', change: 'Nécessitent votre attention' },
]

export default function Dashboard({ onNavigate }) {
  const { hasPermission, hasAnyPermission } = useAuth()
  const canViewReports = hasPermission('Reports.View')
  const canViewBooks = hasPermission('Books.View')
  const [stats, setStats] = useState(null)
  const [books, setBooks] = useState([])
  const [monthly, setMonthly] = useState([])
  const [errors, setErrors] = useState([])

  useEffect(() => {
    let active = true
    const requests = []
    if (canViewReports) {
      requests.push(['statistiques', api.dashboard(), setStats])
      requests.push(['activité mensuelle', api.monthly(), (result) => setMonthly(Array.isArray(result) ? result : [])])
    }
    if (canViewBooks) {
      requests.push(['catalogue', api.list('books'), (result) => setBooks(Array.isArray(result) ? result.slice(0, 5) : [])])
    }

    Promise.allSettled(requests.map(([, request]) => request)).then((results) => {
      if (!active) return
      const nextErrors = []
      results.forEach((result, index) => {
        if (result.status === 'fulfilled') requests[index][2](result.value)
        else nextErrors.push(`Chargement ${requests[index][0]} : ${result.reason.message}`)
      })
      setErrors(nextErrors)
    })
    return () => { active = false }
  }, [canViewBooks, canViewReports])

  const rows = books.map((book) => [
    book.title,
    book.author?.fullName || [book.author?.firstName, book.author?.lastName].filter(Boolean).join(' ') || '—',
    book.category?.name || '—',
    Number(book.availableCopies) > 0 ? 'Disponible' : 'Indisponible',
    book.isbn || '—',
  ])
  const maxLoans = Math.max(1, ...monthly.map((month) => month.loans || 0))
  const [today] = useState(() => new Intl.DateTimeFormat('fr-FR', { weekday: 'long' }).format(new Date()).toLocaleUpperCase('fr-FR'))
  const shortcuts = [
    { id: 'books', label: 'Ouvrages', hint: 'Gérer le catalogue', icon: '▤', tone: 'violet', permissions: ['Books.View', 'Books.Create', 'Books.Edit', 'Books.Delete'] },
    { id: 'students', label: 'Lecteurs', hint: 'Gérer les profils inscrits', icon: '♙', tone: 'blue', permissions: ['Students.View', 'Students.Create', 'Students.Edit', 'Students.Delete'] },
    { id: 'loans', label: 'Emprunts', hint: 'Suivre les sorties en cours', icon: '⇄', tone: 'amber', permissions: ['Loans.View', 'Loans.Create', 'Loans.Return'] },
    { id: 'subscriptions', label: 'Abonnements', hint: 'Consulter les adhésions', icon: '◷', tone: 'green', permissions: ['Subscriptions.View'] },
    { id: 'statistics', label: 'Rapports', hint: 'Explorer les indicateurs', icon: '▥', tone: 'green', permissions: ['Reports.View'] },
  ].filter((item) => hasAnyPermission(...item.permissions))

  return <div className="dashboard-page">
    <section className="welcome-row"><div><div className="eyebrow">{today} · ESPACE DE GESTION</div><h1>Bonjour, <span>bienvenue.</span></h1><p>Voici ce qui se passe dans votre bibliothèque aujourd’hui.</p></div><button className="outline-action" onClick={() => window.print()}><span>↧</span> Exporter le rapport</button></section>
    {errors.map((error) => <div className="inline-warning" key={error}>{error}</div>)}
    {canViewReports && <section className="stats-grid">{cards.map((card) => <article className="stat-card" key={card.key}><div className="stat-top"><span className={`stat-icon ${card.tone}`}>{card.icon}</span><span className="stat-menu">···</span></div><p className="stat-label">{card.label}</p><div className="stat-value">{stats ? (stats[card.key] ?? 0).toLocaleString('fr-FR') : '—'}</div><div className="stat-foot"><span className={`stat-trend ${card.tone}`}>↗</span>{card.change}</div></article>)}</section>}
    {(canViewReports || shortcuts.length > 0) && <section className="dashboard-grid">
      {canViewReports && <article className="panel activity-panel"><div className="panel-heading"><div><span className="section-kicker">VOTRE BIBLIOTHÈQUE</span><h2>Activité des emprunts</h2></div><span className="select-control">7 derniers mois</span></div><div className="chart-summary"><strong>{stats?.totalLoans?.toLocaleString('fr-FR') ?? '—'}</strong><span>emprunts enregistrés</span><span className="summary-change">✦ Par mois</span></div><div className="chart-area"><div className="chart-y"><span>{maxLoans}</span><span>{Math.round(maxLoans * .75)}</span><span>{Math.round(maxLoans * .5)}</span><span>{Math.round(maxLoans * .25)}</span><span>0</span></div><div className="bar-chart" aria-label="Nombre d’emprunts par mois">{monthly.map((month, index) => <div className="bar-column" key={`${month.year}-${month.month}`} title={`${month.loans} emprunts`}><span className={index === monthly.length - 1 ? 'bar-highlight' : ''} style={{ height: `${Math.max(month.loans ? 5 : 0, (month.loans / maxLoans) * 100)}%` }} /></div>)}</div></div><div className="chart-days">{monthly.map((month) => <span key={`${month.year}-${month.month}`}>{new Intl.DateTimeFormat('fr-FR', { month: 'short' }).format(new Date(month.year, month.month - 1, 1))}</span>)}</div></article>}
      {shortcuts.length > 0 && <article className="panel quick-panel"><div className="panel-heading"><div><span className="section-kicker">RACCOURCIS</span><h2>Accès rapide</h2></div><span className="quick-more">···</span></div><div className="quick-list">{shortcuts.map((item) => <button key={item.id} onClick={() => onNavigate(item.id)}><span className={`quick-icon ${item.tone}`}>{item.icon}</span><span><strong>{item.label}</strong><small>{item.hint}</small></span><b>↗</b></button>)}</div></article>}
    </section>}
    {canViewBooks && <section className="panel recent-panel"><div className="panel-heading"><div><span className="section-kicker">CATALOGUE</span><h2>Ouvrages</h2></div><button className="text-action" onClick={() => onNavigate('books')}>Tout le catalogue <span>→</span></button></div><div className="table-scroll"><table><thead><tr><th>TITRE</th><th>AUTEUR</th><th>CATÉGORIE</th><th>DISPONIBILITÉ</th><th>RÉFÉRENCE</th></tr></thead><tbody>{rows.length ? rows.map((row, index) => <tr key={`${row[0]}-${index}`}><td><span className={`book-thumb thumb-${index % 4}`}>▤</span><strong>{row[0]}</strong></td><td>{row[1]}</td><td><span className="category-pill">{row[2]}</span></td><td><span className={`availability ${row[3] === 'Disponible' ? 'available' : 'unavailable'}`}><i />{row[3]}</span></td><td className="reference">{row[4]}</td></tr>) : <tr><td className="empty-state" colSpan="5">Aucun ouvrage à afficher.</td></tr>}</tbody></table></div></section>}
  </div>
}
