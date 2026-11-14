import { useCallback, useEffect, useMemo, useState } from 'react'
import { api } from '../services/api.js'
import { AuthContext } from './authContext.js'

function readStoredUser() {
  try { return JSON.parse(localStorage.getItem('library_user') || 'null') }
  catch { return null }
}

function getUserIdFromToken(token) {
  try {
    const encodedPayload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const paddedPayload = encodedPayload.padEnd(encodedPayload.length + ((4 - encodedPayload.length % 4) % 4), '=')
    const payload = JSON.parse(atob(paddedPayload))
    return payload.userId || payload.nameid || payload.sub || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || null
  } catch {
    return null
  }
}

function permissionNames(assignments) {
  if (!Array.isArray(assignments)) return []
  return assignments
    .map((assignment) => typeof assignment === 'string'
      ? assignment
      : assignment.permission?.name ?? assignment.Permission?.Name)
    .filter((name) => typeof name === 'string')
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('library_token'))
  const [user, setUser] = useState(readStoredUser)
  const [permissions, setPermissions] = useState([])
  const [permissionsLoaded, setPermissionsLoaded] = useState(() => {
    const storedToken = localStorage.getItem('library_token')
    const storedUser = readStoredUser()
    return !storedToken || storedUser?.roles?.includes('Admin') === true || !(storedUser?.userId || getUserIdFromToken(storedToken))
  })
  const [permissionsError, setPermissionsError] = useState('')

  const userId = user?.userId || getUserIdFromToken(token || '')
  const isAdmin = user?.roles?.some((role) => role === 'Admin') ?? false
  const missingUserId = Boolean(token && !isAdmin && !userId)
  const effectivePermissionsLoaded = permissionsLoaded || !token || isAdmin || missingUserId
  const effectivePermissionsError = permissionsError || (missingUserId ? 'Identifiant utilisateur absent de la session. Veuillez vous reconnecter.' : '')

  useEffect(() => {
    if (!token) {
      return
    }

    if (isAdmin || !userId) {
      return
    }

    let active = true
    api.userPermissions(userId)
      .then((assignments) => { if (active) setPermissions(permissionNames(assignments)) })
      .catch((error) => {
        if (active) {
          setPermissions([])
          setPermissionsError(error.message || 'Impossible de charger les permissions.')
        }
      })
      .finally(() => { if (active) setPermissionsLoaded(true) })

    return () => { active = false }
  }, [token, userId, isAdmin])

  const signIn = useCallback((session) => {
    localStorage.setItem('library_token', session.token)
    const profile = {
      userId: session.userId || getUserIdFromToken(session.token),
      fullName: session.fullName,
      email: session.email,
      roles: Array.isArray(session.roles) ? session.roles : [],
    }
    localStorage.setItem('library_user', JSON.stringify(profile))
    setPermissions([])
    const isAdmin = profile.roles.includes('Admin')
    setPermissionsLoaded(isAdmin || !profile.userId)
    setPermissionsError(profile.userId ? '' : 'Identifiant utilisateur absent de la session. Veuillez vous reconnecter.')
    setUser(profile)
    setToken(session.token)
  }, [])

  const signOut = useCallback(() => {
    localStorage.removeItem('library_token')
    localStorage.removeItem('library_user')
    setToken(null)
    setUser(null)
    setPermissions([])
    setPermissionsLoaded(true)
    setPermissionsError('')
  }, [])

  const hasPermission = useCallback((permission) => (
    isAdmin || permissions.includes(permission)
  ), [isAdmin, permissions])

  const hasAnyPermission = useCallback((...requiredPermissions) => (
    isAdmin || requiredPermissions.some((permission) => permissions.includes(permission))
  ), [isAdmin, permissions])

  const value = useMemo(() => ({
    token, user, userId, permissions, permissionsLoaded: effectivePermissionsLoaded, permissionsError: effectivePermissionsError,
    isAdmin, hasPermission, hasAnyPermission, signIn, signOut,
  }), [token, user, userId, permissions, effectivePermissionsLoaded, effectivePermissionsError, isAdmin, hasPermission, hasAnyPermission, signIn, signOut])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

