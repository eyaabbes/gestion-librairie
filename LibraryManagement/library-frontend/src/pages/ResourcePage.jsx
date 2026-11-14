import { useCallback, useEffect, useMemo, useState } from 'react'
import { useAuth } from '../context/useAuth.js'
import { api } from '../services/api.js'
import EmployeePermissions from '../components/EmployeePermissions.jsx'

const definitions = {
  books: {
    title: 'Catalogue des livres', description: 'Retrouvez les ouvrages enregistrés dans votre bibliothèque.', endpoint: 'books',
    search: ['title', 'isbn', 'authorName', 'category'],
    columns: [['title', 'Titre'], ['authorName', 'Auteur'], ['category', 'Catégorie'], ['isbn', 'ISBN'], ['availableCopies', 'Exemplaires'], ['price', 'Prix']],
    fields: [
      { name: 'title', label: 'Titre', required: true }, { name: 'isbn', label: 'ISBN', required: true },
      { name: 'publicationYear', label: 'Année de publication', type: 'number', required: true },
      { name: 'availableCopies', label: 'Exemplaires disponibles', type: 'number', required: true },
      { name: 'price', label: 'Prix', type: 'number', required: true },
      { name: 'description', label: 'Description', type: 'textarea', required: true },
      { name: 'authorId', label: 'ID de l’auteur', type: 'number', required: true },
      { name: 'categoryId', label: 'ID de la catégorie', type: 'number', required: true },
    ],
  },
  authors: {
    title: 'Répertoire des auteurs', description: 'Les auteurs référencés dans votre catalogue.', endpoint: 'authors', search: ['firstName', 'lastName'],
    columns: [['firstName', 'Prénom'], ['lastName', 'Nom']],
    fields: [{ name: 'firstName', label: 'Prénom', required: true }, { name: 'lastName', label: 'Nom', required: true }],
  },
  categories: {
    title: 'Catégories', description: 'Organisez les ouvrages par thème et par genre.', endpoint: 'categories', search: ['name'],
    columns: [['name', 'Catégorie']], fields: [{ name: 'name', label: 'Nom', required: true }],
  },
  students: {
    title: 'Lecteurs et étudiants', description: 'Consultez les profils des lecteurs inscrits.', endpoint: 'students', search: ['fullName', 'email'],
    columns: [['fullName', 'Nom complet'], ['email', 'E-mail']],
    fields: [{ name: 'fullName', label: 'Nom complet', required: true }, { name: 'email', label: 'Adresse e-mail', type: 'email', required: true }],
  },
  employees: {
    title: 'Équipe de la bibliothèque', description: 'Retrouvez les membres du personnel.', endpoint: 'employees', search: ['fullName', 'email', 'jobTitle'],
    columns: [['fullName', 'Nom complet'], ['email', 'E-mail'], ['jobTitle', 'Fonction'], ['isActive', 'Actif']],
    fields: [
      { name: 'fullName', label: 'Nom complet', required: true }, { name: 'email', label: 'Adresse e-mail', type: 'email', required: true },
      { name: 'jobTitle', label: 'Fonction', required: true }, { name: 'isActive', label: 'Employé actif', type: 'checkbox' },
    ],
  },
  loans: {
    title: 'Suivi des emprunts', description: 'Visualisez les emprunts et leur état.', endpoint: 'loans', search: ['bookTitle', 'studentName', 'loanStatus'],
    columns: [['bookTitle', 'Ouvrage'], ['studentName', 'Lecteur'], ['borrowDate', 'Date de sortie'], ['dueDate', 'Date de retour prévue'], ['loanStatus', 'Statut']],
    fields: [{ name: 'bookId', label: 'ID du livre', type: 'number', required: true }, { name: 'studentId', label: 'ID de l’étudiant', type: 'number', required: true }],
  },
}

const prefixByResource = { books: 'Books', authors: 'Authors', categories: 'Categories', students: 'Students', employees: 'Employees', loans: 'Loans' }

function getValue(item, key) {
  if (key === 'authorName') return [item?.author?.firstName, item?.author?.lastName].filter(Boolean).join(' ') || '—'
  if (key === 'loanStatus') {
    if (item?.returnDate) return 'Rendu'
    if (item?.dueDate && new Date(item.dueDate).getTime() < Date.now()) return 'En retard'
    return 'En cours'
  }
  const value = item?.[key]
  if (value && typeof value === 'object') return value.fullName || value.name || value.title || '—'
  if (value == null || value === '') return '—'
  if (typeof value === 'boolean') return value ? 'Oui' : 'Non'
  if (key.toLowerCase().includes('date')) {
    const date = new Date(value)
    return Number.isNaN(date.valueOf()) ? String(value) : new Intl.DateTimeFormat('fr-FR').format(date)
  }
  return String(value)
}

function emptyForm(fields, item = null) {
  return Object.fromEntries(fields.map(({ name, type }) => [name, item?.[name] ?? (type === 'checkbox' ? true : '')]))
}

export default function ResourcePage({ resource, onBookSelect }) {
  const definition = definitions[resource]
  const { hasPermission, isAdmin } = useAuth()
  const prefix = prefixByResource[resource]
  const canView = hasPermission(`${prefix}.View`)
  const canCreate = hasPermission(`${prefix}.Create`)
  const canEdit = resource !== 'loans' && hasPermission(`${prefix}.Edit`)
  const canDelete = resource !== 'loans' && hasPermission(`${prefix}.Delete`)
  const canReturn = resource === 'loans' && hasPermission('Loans.Return')
  const [items, setItems] = useState([])
  const [search, setSearch] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(canView)
  const [saving, setSaving] = useState(false)
  const [dialog, setDialog] = useState(null)
  const [form, setForm] = useState(() => emptyForm(definition.fields))
  const [employeePassword, setEmployeePassword] = useState('Employee@123456')
  const [selectedImage, setSelectedImage] = useState(null)
  const [imagePreview, setImagePreview] = useState('')

  useEffect(() => {
    if (!selectedImage) {
      setImagePreview(resource === 'books' ? dialog?.item?.imageUrl || '' : '')
      return undefined
    }
    const previewUrl = URL.createObjectURL(selectedImage)
    setImagePreview(previewUrl)
    return () => URL.revokeObjectURL(previewUrl)
  }, [dialog, resource, selectedImage])

  const reload = useCallback(async () => {
    if (!canView) return
    setLoading(true)
    setError('')
    try {
      const result = await api.list(definition.endpoint)
      setItems(Array.isArray(result) ? result : [])
    } catch (requestError) {
      setError(requestError.message)
    } finally {
      setLoading(false)
    }
  }, [canView, definition.endpoint])

  useEffect(() => {
    if (!canView) return undefined
    let active = true
    api.list(definition.endpoint)
      .then((result) => { if (active) setItems(Array.isArray(result) ? result : []) })
      .catch((requestError) => { if (active) setError(requestError.message) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [canView, definition.endpoint])

  const columns = useMemo(() => definition.columns, [definition])
  const visibleItems = items.filter((item) => definition.search.some((key) => getValue(item, key).toLowerCase().includes(search.toLowerCase())))
  const hasRowActions = canView && (canEdit || canDelete || canReturn)

  function openForm(mode, item = null) {
    setForm(emptyForm(definition.fields, item))
    setSelectedImage(null)
    if (resource === 'employees' && mode === 'create') setEmployeePassword('Employee@123456')
    setDialog({ mode, item })
    setError('')
  }

  function selectBookImage(event) {
    const file = event.target.files?.[0] || null
    if (!file) return
    if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type)) {
      setError('Choisissez une image JPG, PNG ou WEBP.')
      event.target.value = ''
      return
    }
    if (file.size > 5 * 1024 * 1024) {
      setError('L’image ne doit pas dépasser 5 Mo.')
      event.target.value = ''
      return
    }
    setError('')
    setSelectedImage(file)
  }

  async function submitForm(event) {
    event.preventDefault()
    setSaving(true)
    setError('')
    const payload = Object.fromEntries(definition.fields.map(({ name, type }) => {
      const value = form[name]
      return [name, type === 'number' ? Number(value) : type === 'checkbox' ? Boolean(value) : value]
    }))
    try {
      if (dialog.mode === 'create') {
        const created = await api.create(definition.endpoint, payload)
        if (resource === 'books' && selectedImage) {
          try {
            const uploadedBook = await api.uploadBookImage(created.id, selectedImage)
            setItems((current) => [...current.filter((item) => item.id !== uploadedBook.id), uploadedBook])
          } catch (uploadError) {
            setDialog({ mode: 'edit', item: created })
            setForm(emptyForm(definition.fields, created))
            setError(`Livre créé, mais l’image n’a pas été envoyée : ${uploadError.message}. Vous pouvez réessayer.`)
            return
          }
        }
        if (resource === 'employees') {
          try {
            await api.createEmployeeAccount(created.id, { email: payload.email, password: employeePassword })
          } catch (accountError) {
            await reload()
            const employee = await api.getEmployee(created.id).catch(() => created)
            setForm(emptyForm(definition.fields, employee))
            setDialog({ mode: 'edit', item: employee })
            setError(`Profil créé, mais le compte n’a pas été créé : ${accountError.message}. Vous pouvez réessayer dans la section des permissions.`)
            return
          }
        }
      } else {
        await api.update(definition.endpoint, dialog.item.id, payload)
        if (resource === 'books' && selectedImage) {
          const uploadedBook = await api.uploadBookImage(dialog.item.id, selectedImage)
          setItems((current) => current.map((item) => item.id === uploadedBook.id ? uploadedBook : item))
        }
      }
      setDialog(null)
      await reload()
    } catch (requestError) {
      setError(requestError.message)
    } finally {
      setSaving(false)
    }
  }

  async function deleteItem(item) {
    if (!window.confirm(`Supprimer « ${getValue(item, definition.columns[0][0])} » ?`)) return
    setError('')
    try { await api.remove(definition.endpoint, item.id); await reload() }
    catch (requestError) { setError(requestError.message) }
  }

  async function returnItem(item) {
    setError('')
    try { await api.returnLoan(item.id); await reload() }
    catch (requestError) { setError(requestError.message) }
  }

  return <section className="resource-page">
    <div className="page-title-row"><div><div className="eyebrow">GESTION DE LA BIBLIOTHÈQUE</div><h1>{definition.title}</h1><p>{definition.description}</p></div><div className="resource-actions">{canCreate && <button className="primary-action" onClick={() => openForm('create')}>＋ <span>Ajouter</span></button>}{canView && <button className="outline-action" onClick={reload} disabled={loading}>↻ <span>Actualiser</span></button>}</div></div>
    {error && <div className="inline-warning" role="alert">{error}</div>}
    {canView ? <>
      <div className="resource-toolbar"><div className="input-wrap search-wrap"><span>⌕</span><input aria-label="Rechercher" placeholder="Rechercher dans la liste…" value={search} onChange={(event) => setSearch(event.target.value)} /></div><span className="result-count">{items.length} résultat{items.length > 1 ? 's' : ''}</span></div>
      <div className="panel resource-table"><div className="table-scroll"><table><thead><tr>{columns.map(([key, label]) => <th key={key}>{label}</th>)}{hasRowActions && <th>ACTIONS</th>}</tr></thead><tbody>{loading ? <tr><td className="empty-state" colSpan={columns.length + Number(hasRowActions)}>Chargement des données…</td></tr> : visibleItems.length ? visibleItems.map((item, index) => <tr key={item.id ?? index}>{columns.map(([key], colIndex) => <td key={key}>{colIndex === 0 && resource === 'books' ? <button type="button" className="book-open-link" onClick={() => onBookSelect?.(item.id)}>{item.imageUrl && <img src={item.imageUrl} alt="" />}{getValue(item, key)}</button> : key === 'loanStatus' ? <span className={`loan-status ${getValue(item, key) === 'Rendu' ? 'returned' : getValue(item, key) === 'En retard' ? 'overdue' : 'ongoing'}`}>{getValue(item, key)}</span> : getValue(item, key)}</td>)}{hasRowActions && <td><div className="row-actions">{canEdit && <button className="row-action" onClick={() => openForm('edit', item)} title="Modifier" aria-label="Modifier">✎</button>}{canDelete && <button className="row-action danger" onClick={() => deleteItem(item)} title="Supprimer" aria-label="Supprimer">⌫</button>}{canReturn && !item.returnDate && <button className="row-action return-action" onClick={() => returnItem(item)} title="Enregistrer le retour">Retour</button>}</div></td>}</tr>) : <tr><td className="empty-state" colSpan={columns.length + Number(hasRowActions)}>{items.length ? 'Aucun résultat pour cette recherche.' : 'Aucune donnée à afficher pour le moment.'}</td></tr>}</tbody></table></div><div className="table-bottom"><span>Affichage de {visibleItems.length} sur {items.length} entrées</span><div><button disabled>←</button><button className="current-page">1</button><button disabled>→</button></div></div></div>
    </> : <div className="panel no-view-panel"><span className="quick-icon violet">⌕</span><div><strong>Consultation non autorisée</strong><p>Vous pouvez effectuer les actions autorisées ci-dessus, mais l’accès à la liste nécessite la permission {prefix}.View.</p></div></div>}

    {dialog && <div className="modal-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget) setDialog(null) }}><section className="form-modal" role="dialog" aria-modal="true" aria-labelledby="resource-form-title"><div className="modal-heading"><div><span className="section-kicker">{definition.title.toLocaleUpperCase('fr-FR')}</span><h2 id="resource-form-title">{dialog.mode === 'create' ? 'Ajouter' : 'Modifier'} : {definition.title.toLocaleLowerCase('fr-FR')}</h2></div><button className="modal-close" onClick={() => setDialog(null)} aria-label="Fermer">×</button></div><form onSubmit={submitForm}><div className="modal-fields">{definition.fields.map((field) => <label className={field.type === 'checkbox' ? 'checkbox-field' : ''} key={field.name}>{field.type === 'checkbox' ? <><input type="checkbox" checked={Boolean(form[field.name])} onChange={(event) => setForm({ ...form, [field.name]: event.target.checked })} />{field.label}</> : field.type === 'textarea' ? <>{field.label}<textarea required={field.required} value={form[field.name]} onChange={(event) => setForm({ ...form, [field.name]: event.target.value })} /></> : <>{field.label}<input type={field.type || 'text'} min={field.type === 'number' ? 0 : undefined} step={field.name === 'price' ? '0.01' : undefined} required={field.required} value={form[field.name]} onChange={(event) => setForm({ ...form, [field.name]: event.target.value })} /></>}</label>)}</div>{resource === 'books' && canEdit && <div className="book-image-upload"><div className="book-image-upload-copy"><strong>Image de couverture</strong><small>JPG, PNG ou WEBP · 5 Mo maximum</small><label className="book-image-choose">Choisir une image<input type="file" accept="image/jpeg,image/png,image/webp" onChange={selectBookImage} /></label></div><div className="book-image-preview">{imagePreview ? <img src={imagePreview} alt="Aperçu de la couverture" /> : <span>Aucune couverture sélectionnée</span>}</div></div>}{resource === 'books' && !canEdit && <p className="book-image-permission-note">La permission Books.Edit est nécessaire pour téléverser une couverture.</p>}{resource === 'employees' && dialog.mode === 'create' && <label className="employee-password-field">Mot de passe initial du compte<input type="password" autoComplete="new-password" minLength={8} required value={employeePassword} onChange={(event) => setEmployeePassword(event.target.value)} /><small>Valeur proposée : Employee@123456. L’employé se connectera avec son e-mail et ce mot de passe.</small></label>}{resource === 'employees' && isAdmin && dialog.mode === 'edit' && <EmployeePermissions employeeId={dialog.item.id} employeeEmail={dialog.item.email} applicationUserId={dialog.item.applicationUserId} />}{error && <div className="form-error" role="alert">{error}</div>}<div className="modal-actions"><button type="button" className="outline-action" onClick={() => setDialog(null)}>Annuler</button><button type="submit" className="primary-action" disabled={saving}>{saving ? 'Enregistrement…' : 'Enregistrer'}</button></div></form></section></div>}
  </section>
}
