import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Book } from '../../core/models/book';
import { BookChanges, NewBook } from '../../core/models/book-requests';
import { BOOK_REPOSITORY } from '../tokens/repository.tokens';
import { BooksStore } from './books.store';

/** Add, update and delete use cases; each refreshes the grid and genre counts on success. */
@Injectable({ providedIn: 'root' })
export class BookCommands {
  private readonly repo = inject(BOOK_REPOSITORY);
  private readonly store = inject(BooksStore);

  add(book: NewBook): Observable<number> {
    return this.repo.create(book).pipe(tap(() => this.store.reload()));
  }

  update(id: number, changes: BookChanges): Observable<Book> {
    return this.repo.update(id, changes).pipe(tap(() => this.store.reload()));
  }

  remove(id: number): Observable<void> {
    return this.repo.delete(id).pipe(tap(() => this.store.reload()));
  }
}
