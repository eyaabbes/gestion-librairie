const API_URL = (import.meta.env.VITE_API_URL || '/api').replace(/\/$/, '')

export async function apiRequest(path, options = {}) {
  const token = localStorage.getItem('library_token')
  const headers = new Headers(options.headers || {})
  if (token) headers.set('Authorization', `Bearer ${token}`)
  if (options.body && !(options.body instanceof FormData)) headers.set('Content-Type', 'application/json')

  const response = await fetch(`${API_URL}${path.startsWith('/') ? path : `/${path}`}`, {
    ...options,
    headers,
  })
  if (response.status === 204) return null
  const contentType = response.headers.get('content-type') || ''
  const payload = contentType.includes('application/json') ? await response.json() : await response.text()
  if (!response.ok) {
    const message = typeof payload === 'string' ? payload : payload?.message || payload?.title || 'Une erreur est survenue.'
    throw new Error(message)
  }
  return payload
}

export const api = {
  login: (email, password) => apiRequest('/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) }),
  userPermissions: (userId) => apiRequest(`/permissions/users/${encodeURIComponent(userId)}/permissions`),
  permissions: () => apiRequest('/permissions'),
  grantUserPermission: (userId, permissionId) => apiRequest(`/permissions/users/${encodeURIComponent(userId)}/permissions/${permissionId}`, { method: 'PUT' }),
  revokeUserPermission: (userId, permissionId) => apiRequest(`/permissions/users/${encodeURIComponent(userId)}/permissions/${permissionId}`, { method: 'DELETE' }),
  dashboard: () => apiRequest('/statistics/dashboard'),
  monthly: () => apiRequest('/statistics/monthly?months=7'),
  getBook: (id) => apiRequest(`/books/${id}`),
  uploadBookImage: (id, file) => {
    const formData = new FormData()
    formData.append('image', file)
    return apiRequest(`/books/${id}/image`, { method: 'POST', body: formData })
  },
  subscriptions: () => apiRequest('/subscriptions'),
  list: (resource) => apiRequest(`/${resource}`),
  create: (resource, payload) => apiRequest(`/${resource}`, { method: 'POST', body: JSON.stringify(payload) }),
  getEmployee: (id) => apiRequest(`/employees/${id}`),
  createEmployeeAccount: (id, payload) => apiRequest(`/employees/${id}/account`, { method: 'POST', body: JSON.stringify(payload) }),
  update: (resource, id, payload) => apiRequest(`/${resource}/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
  remove: (resource, id) => apiRequest(`/${resource}/${id}`, { method: 'DELETE' }),
  returnLoan: (id) => apiRequest(`/loans/${id}/return`, { method: 'PUT' }),
}

