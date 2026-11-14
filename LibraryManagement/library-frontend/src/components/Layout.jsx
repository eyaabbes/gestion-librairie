import { useState } from 'react'
import Sidebar from './Sidebar.jsx'
import Header from './Header.jsx'

export default function Layout({ activePage, onNavigate, children }) {
  const [menuOpen, setMenuOpen] = useState(false)
  return <div className="app-shell">
    <div className={`sidebar-wrap ${menuOpen ? 'open' : ''}`}><Sidebar activePage={activePage} onNavigate={(page) => { onNavigate(page); setMenuOpen(false) }} /><button className="sidebar-close" aria-label="Fermer le menu" onClick={() => setMenuOpen(false)}>×</button></div>
    {menuOpen && <button className="mobile-scrim" aria-label="Fermer le menu" onClick={() => setMenuOpen(false)} />}
    <main className="main-area"><Header activePage={activePage} onMenuClick={() => setMenuOpen(true)} /><div className="page-area">{children}</div><footer className="page-footer"><span>© 2026 Bookkeep Library</span><span>Conçu pour faciliter la gestion de votre bibliothèque <b>✳</b></span></footer></main>
  </div>
}
