import { useState } from 'react'
import { useAuth } from '../context/useAuth.js'

const titles = { dashboard: 'Vue d’ensemble', books: 'Livres', authors: 'Auteurs', categories: 'Catégories', students: 'Étudiants', employees: 'Employés', loans: 'Emprunts', subscriptions: 'Abonnements', penalties: 'Pénalités', statistics: 'Statistiques' }

export default function Header({ activePage, onMenuClick }) {
  const { user, signOut } = useAuth()
  const [today] = useState(() => new Intl.DateTimeFormat('fr-FR', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' }).format(new Date()))
  const initials = (user?.fullName || user?.email || 'AD').split(/[\s@.]+/).filter(Boolean).slice(0, 2).map((part) => part[0].toUpperCase()).join('')
  return <header className="topbar">
    <div className="topbar-left"><button className="mobile-menu" onClick={onMenuClick} aria-label="Ouvrir le menu">☰</button><div className="breadcrumbs"><span>Bibliothèque</span><span className="crumb-sep">/</span><strong>{titles[activePage]}</strong></div></div>
    <div className="topbar-right"><div className="date-display"><span className="date-icon">▦</span><span>{today}</span></div><button className="icon-button notification-button" aria-label="Notifications">♧<i /></button><div className="profile"><div className="avatar">{initials}</div><div className="profile-copy"><strong>{user?.fullName || 'Administrateur'}</strong><small>{user?.roles?.[0] || 'Administration'}</small></div><span className="chevron">⌄</span></div><button className="logout-button" type="button" onClick={signOut}><span aria-hidden="true">↪</span><span>Déconnexion</span></button></div>
  </header>
}
