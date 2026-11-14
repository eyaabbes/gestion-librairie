import { useState } from 'react'
import { AuthProvider } from './context/AuthContext.jsx'
import { useAuth } from './context/useAuth.js'
import Layout from './components/Layout.jsx'
import Login from './pages/Login.jsx'
import Dashboard from './pages/Dashboard.jsx'
import Books from './pages/Books/Books.jsx'
import Authors from './pages/Authors/Authors.jsx'
import Categories from './pages/Categories/Categories.jsx'
import Students from './pages/Students/Students.jsx'
import Employees from './pages/Employees/Employees.jsx'
import Loans from './pages/Loans/Loans.jsx'
import Subscriptions from './pages/Subscriptions/Subscriptions.jsx'
import Penalties from './pages/Penalties/Penalties.jsx'
import Statistics from './pages/Statistics/Statistics.jsx'

const pages = {
  dashboard: Dashboard,
  books: Books,
  authors: Authors,
  categories: Categories,
  students: Students,
  employees: Employees,
  loans: Loans,
  subscriptions: Subscriptions,
  penalties: Penalties,
  statistics: Statistics,
}

function Workspace() {
  const { token, permissionsLoaded, permissionsError } = useAuth()
  const [activePage, setActivePage] = useState('dashboard')

  if (!token) return <Login />
  if (!permissionsLoaded) return <main className="auth-loading">Chargement de vos accès…</main>

  const Page = pages[activePage] ?? Dashboard
  return (
    <Layout activePage={activePage} onNavigate={setActivePage}>
      {permissionsError && <div className="inline-warning auth-warning">{permissionsError}</div>}
      <Page onNavigate={setActivePage} />
    </Layout>
  )
}

export default function App() {
  return <AuthProvider><Workspace /></AuthProvider>
}
