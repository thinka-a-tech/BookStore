import { Observable } from 'rxjs';
import { Book } from '../models/book';
import { BookChanges, BookListQuery, NewBook } from '../models/book-requests';
import { GenreCount } from '../models/genre-count';
import { PagedResult } from '../models/paged-result';

/** Port for the store's own books (SQLite via the API). Failures are emitted as AppError. */
export interface BookRepository {
  list(query: BookListQuery): Observable<PagedResult<Book>>;
  get(id: number): Observable<Book>;
  genreCounts(): Observable<readonly GenreCount[]>;
  create(book: NewBook): Observable<number>;
  update(id: number, changes: BookChanges): Observable<Book>;
  delete(id: number): Observable<void>;
}
