import { useEffect, useState } from 'react'
import { api } from '../../services/api.js'

function formatMoney(value) {
  return value == null ? 'Prix non renseigné' : `${Number(value).toLocaleString('fr-TN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}

export default function BookDetails({ bookId, onBack }) {
  const [book, setBook] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let active = true
    setLoading(true)
    api.getBook(bookId)
      .then((result) => { if (active) setBook(result) })
      .catch((requestError) => { if (active) setError(requestError.message) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [bookId])

  if (loading) return <section className="resource-page"><p className="book-detail-message">Chargement du livre…</p></section>
  if (error || !book) return <section className="resource-page"><button className="outline-action" onClick={onBack}>← Retour au catalogue</button><div className="inline-warning book-detail-error">{error || 'Livre introuvable.'}</div></section>

  const author = [book.author?.firstName, book.author?.lastName].filter(Boolean).join(' ')

  return <section className="resource-page book-detail-page">
    <button className="outline-action book-back-button" onClick={onBack}>← Retour au catalogue</button>
    <article className="panel book-detail-card">
      <div className="book-detail-cover">
        {book.imageUrl ? <img src={book.imageUrl} alt={`Couverture de ${book.title}`} /> : <div className="book-cover-placeholder">▤</div>}
      </div>
      <div className="book-detail-content">
        <span className="eyebrow">DÉTAILS DU LIVRE</span>
        <h1>{book.title}</h1>
        <p className="book-detail-byline">{author || 'Auteur non renseigné'}{book.category?.name ? ` · ${book.category.name}` : ''}</p>
        <p className="book-detail-price">{formatMoney(book.price)}</p>
        <p className="book-detail-description">{book.description || 'Aucune description disponible pour ce livre.'}</p>
        <dl className="book-detail-facts">
          <div><dt>ISBN</dt><dd>{book.isbn || '—'}</dd></div>
          <div><dt>Année de publication</dt><dd>{book.publicationYear || '—'}</dd></div>
          <div><dt>Exemplaires disponibles</dt><dd>{book.availableCopies ?? '—'}</dd></div>
          <div><dt>Catégorie</dt><dd>{book.category?.name || '—'}</dd></div>
          <div><dt>Auteur</dt><dd>{author || '—'}</dd></div>
        </dl>
      </div>
    </article>
  </section>
}
