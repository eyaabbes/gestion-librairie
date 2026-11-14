import { useEffect, useState } from 'react'
import { api } from '../services/api.js'

export default function EmployeePermissions({ employeeId, employeeEmail, applicationUserId }) {
  const [available, setAvailable] = useState([])
  const [assigned, setAssigned] = useState([])
  const [linkedUserId, setLinkedUserId] = useState(applicationUserId)
  const [loading, setLoading] = useState(Boolean(applicationUserId))
  const [pendingId, setPendingId] = useState(null)
  const [initialPassword, setInitialPassword] = useState('Employee@123456')
  const [creatingAccount, setCreatingAccount] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    setLinkedUserId(applicationUserId)
  }, [applicationUserId])

  useEffect(() => {
    if (!linkedUserId) return undefined

    let active = true
    setLoading(true)
    setError('')
    Promise.all([api.permissions(), api.userPermissions(linkedUserId)])
      .then(([permissions, names]) => {
        if (!active) return
        setAvailable(Array.isArray(permissions) ? permissions : [])
        setAssigned(Array.isArray(names) ? names.filter((name) => typeof name === 'string') : [])
      })
      .catch((requestError) => {
        if (active) setError(requestError.message || 'Impossible de charger les permissions.')
      })
      .finally(() => { if (active) setLoading(false) })

    return () => { active = false }
  }, [linkedUserId])

  async function createAccount(event) {
    event.preventDefault()
    setCreatingAccount(true)
    setError('')
    try {
      await api.createEmployeeAccount(employeeId, { email: employeeEmail, password: initialPassword })
      const employee = await api.getEmployee(employeeId)
      setLinkedUserId(employee.applicationUserId)
    } catch (requestError) {
      setError(requestError.message || 'Impossible de créer le compte de cet employé.')
    } finally {
      setCreatingAccount(false)
    }
  }

  async function togglePermission(permission, checked) {
    setPendingId(permission.id)
    setError('')
    try {
      if (checked) {
        await api.grantUserPermission(linkedUserId, permission.id)
        setAssigned((current) => current.includes(permission.name) ? current : [...current, permission.name])
      } else {
        await api.revokeUserPermission(linkedUserId, permission.id)
        setAssigned((current) => current.filter((name) => name !== permission.name))
      }
    } catch (requestError) {
      setError(requestError.message || 'La modification de cette permission a échoué.')
    } finally {
      setPendingId(null)
    }
  }

  return <section className="employee-permissions" aria-labelledby="employee-permissions-title">
    <div className="employee-permissions-heading">
      <div>
        <h3 id="employee-permissions-title">Permissions de connexion</h3>
        <p>Les changements sont enregistrés immédiatement.</p>
      </div>
      <span>{assigned.length} attribuée{assigned.length > 1 ? 's' : ''}</span>
    </div>

    {!linkedUserId
      ? <form className="employee-account-form" onSubmit={createAccount}>
        <p className="employee-permissions-message">Aucun compte de connexion n’est associé à cet employé. Créez son compte pour qu’il puisse se connecter.</p>
        <label>Mot de passe initial<input type="password" autoComplete="new-password" minLength={8} required value={initialPassword} onChange={(event) => setInitialPassword(event.target.value)} /></label>
        <button className="primary-action" type="submit" disabled={creatingAccount}>{creatingAccount ? 'Création…' : 'Créer le compte Employee'}</button>
      </form>
      : loading
        ? <p className="employee-permissions-message">Chargement des permissions…</p>
        : <div className="employee-permissions-list">
          {available.map((permission) => <label className="employee-permission-item" key={permission.id}>
            <input
              type="checkbox"
              checked={assigned.includes(permission.name)}
              disabled={pendingId !== null}
              onChange={(event) => togglePermission(permission, event.target.checked)}
            />
            <span><strong>{permission.name}</strong><small>{permission.description}</small></span>
            {pendingId === permission.id && <i>Enregistrement…</i>}
          </label>)}
          {!available.length && <p className="employee-permissions-message">Aucune permission disponible.</p>}
        </div>}

    {error && <p className="employee-permissions-error" role="alert">{error}</p>}
  </section>
}
