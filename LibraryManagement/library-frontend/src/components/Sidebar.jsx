import { useAuth } from '../context/useAuth.js'

const groups = [
  { label: 'ESPACE', items: [{ id: 'dashboard', label: 'Vue d’ensemble', icon: '◫' }] },
  { label: 'COLLECTION', items: [
    { id: 'books', label: 'Livres', icon: '▤', permissions: ['Books.View', 'Books.Create', 'Books.Edit', 'Books.Delete'] },
    { id: 'authors', label: 'Auteurs', icon: '✳', permissions: ['Authors.View', 'Authors.Create', 'Authors.Edit', 'Authors.Delete'] },
    { id: 'categories', label: 'Catégories', icon: '▦', permissions: ['Categories.View', 'Categories.Create', 'Categories.Edit', 'Categories.Delete'] },
  ] },
  { label: 'ACTIVITÉ', items: [
    { id: 'loans', label: 'Emprunts', icon: '⇄', permissions: ['Loans.View', 'Loans.Create', 'Loans.Return'] },
    { id: 'students', label: 'Étudiants', icon: '♙', permissions: ['Students.View', 'Students.Create', 'Students.Edit', 'Students.Delete'] },
    { id: 'employees', label: 'Employés', icon: '♧', permissions: ['Employees.View', 'Employees.Create', 'Employees.Edit', 'Employees.Delete'] },
    { id: 'subscriptions', label: 'Abonnements', icon: '◷', permissions: ['Subscriptions.View'] },
  ] },
  { label: 'GESTION', items: [
    { id: 'statistics', label: 'Statistiques', icon: '▥', permissions: ['Reports.View'] },
  ] },
]

export default function Sidebar({ activePage, onNavigate }) {
  const { hasAnyPermission } = useAuth()
  const visibleGroups = groups.map((group) => ({
    ...group,
    items: group.items.filter((item) => !item.permissions || hasAnyPermission(...item.permissions)),
  })).filter((group) => group.items.length > 0)

  return (
    <aside className="sidebar">
      <a className="brand" href="#home" onClick={(event) => { event.preventDefault(); onNavigate('dashboard') }}>
        <span className="brand-mark">b<span>.</span></span>
        <span className="brand-copy"><strong>BOOKKEEP</strong><small>LIBRARY MANAGEMENT</small></span>
      </a>
      <div className="branch-card"><span className="branch-icon">⌂</span><span><strong>Bibliothèque centrale</strong><small>Établissement principal</small></span><span className="chevron">⌄</span></div>
      <nav className="side-nav" aria-label="Navigation principale">
        {visibleGroups.map((group) => <div className="nav-group" key={group.label}>
          <p className="nav-label">{group.label}</p>
          {group.items.map((item) => <button key={item.id} className={`nav-item ${activePage === item.id ? 'active' : ''}`} onClick={() => onNavigate(item.id)}>
            <span className="nav-icon">{item.icon}</span><span>{item.label}</span>
          </button>)}
        </div>)}
      </nav>
      <div className="sidebar-bottom"><div className="support-card"><span className="support-spark">✦</span><strong>Besoin d’aide ?</strong><p>Consultez le guide de votre espace.</p><button onClick={() => window.open('mailto:support@bookkeep.local')}>Contacter le support <span>↗</span></button></div><div className="sidebar-foot"><span className="online-dot" /> Système opérationnel <span className="version">v1.0</span></div></div>
    </aside>
  )
}
