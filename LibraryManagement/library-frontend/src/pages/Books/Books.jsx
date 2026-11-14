import ResourcePage from '../ResourcePage.jsx'
import { useState } from 'react'
import BookDetails from './BookDetails.jsx'

export default function Books() {
  const [selectedBookId, setSelectedBookId] = useState(null)
  return selectedBookId
    ? <BookDetails bookId={selectedBookId} onBack={() => setSelectedBookId(null)} />
    : <ResourcePage resource="books" onBookSelect={setSelectedBookId} />
}
