import { useState } from 'react'
import { api } from '../services/api.js'
import { useAuth } from '../context/useAuth.js'

export default function Login() {
  const { signIn } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setBusy(true)
    try { signIn(await api.login(email, password)) }
    catch (requestError) { setError(requestError.message || 'Connexion impossible.') }
    finally { setBusy(false) }
  }

  return <main className="login-screen"><div className="login-art"><div className="login-art-top"><span className="brand-mark">b<span>.</span></span><span>BOOKKEEP <i>LIBRARY MANAGEMENT</i></span></div><div className="art-orbit orbit-one" /><div className="art-orbit orbit-two" /><div className="book-stack"><div className="book book-a" /><div className="book book-b" /><div className="book book-c" /><div className="book book-d" /><div className="book book-e" /></div><div className="art-quote"><span>«</span><p>Une bibliothèque est un lieu<br />où les histoires prennent vie.</p><small>VOTRE ESPACE, VOTRE HISTOIRE</small></div><div className="art-glow" /></div><section className="login-panel"><div className="login-form-wrap"><div className="eyebrow"><span /> ESPACE ADMINISTRATEUR</div><h1>Ravi de vous<br />revoir<span>.</span></h1><p className="login-intro">Connectez-vous pour accéder à votre espace de gestion.</p><form onSubmit={handleSubmit}><label htmlFor="email">Adresse e-mail</label><div className="input-wrap"><span>✉</span><input id="email" type="email" autoComplete="username" placeholder="nom@bibliotheque.tn" value={email} onChange={(event) => setEmail(event.target.value)} required /></div><div className="password-label"><label htmlFor="password">Mot de passe</label><button type="button" onClick={() => { const input = document.getElementById('password'); input.type = input.type === 'password' ? 'text' : 'password' }}>Afficher</button></div><div className="input-wrap"><span>⌑</span><input id="password" type="password" autoComplete="current-password" placeholder="Votre mot de passe" value={password} onChange={(event) => setPassword(event.target.value)} required /></div>{error && <div className="form-error" role="alert">{error}</div>}<button className="login-submit" disabled={busy}>{busy ? 'Connexion…' : 'Se connecter'} <span>→</span></button></form><p className="login-hint">Accès réservé au personnel de la bibliothèque.</p></div><div className="login-footer"><span>© 2026 Bookkeep</span><span>Besoin d’aide ? <a href="mailto:support@bookkeep.local">Contactez-nous</a></span></div></section></main>
}
