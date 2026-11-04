import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { Book } from '../../core/models/book';
import { BookChanges, BookListQuery, NewBook } from '../../core/models/book-requests';
import { GenreCount } from '../../core/models/genre-count';
import { PagedResult } from '../../core/models/paged-result';
import { BookRepository } from '../../core/repositories/book.repository';
import { API_BASE_URL } from '../config/api-base-url';
import { BookDto, GenreCountDto, PagedResultDto } from '../dto/api.dto';
import { toAppError } from '../http/to-app-error';
import { toBook, toGenreCount, toPagedBooks } from '../mappers/api.mappers';

@Injectable()
export class HttpBookRepository implements BookRepository {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_BASE_URL)}/books`;

  list(query: BookListQuery): Observable<PagedResult<Book>> {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize)
      .set('sortBy', query.sortBy)
      .set('descending', query.descending);
    if (query.genre) {
      params = params.set('genre', query.genre);
    }
    return this.http.get<PagedResultDto<BookDto>>(this.url, { params }).pipe(map(toPagedBooks), this.fail());
  }

  get(id: number): Observable<Book> {
    return this.http.get<BookDto>(`${this.url}/${id}`).pipe(map(toBook), this.fail());
  }

  genreCounts(): Observable<readonly GenreCount[]> {
    return this.http.get<GenreCountDto[]>(`${this.url}/genres`).pipe(map((d) => d.map(toGenreCount)), this.fail());
  }

  create(book: NewBook): Observable<number> {
    return this.http.post<number>(this.url, book).pipe(this.fail());
  }

  update(id: number, changes: BookChanges): Observable<Book> {
    return this.http.put<BookDto>(`${this.url}/${id}`, changes).pipe(map(toBook), this.fail());
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`).pipe(this.fail());
  }

  private fail<T>() {
    return catchError<T, Observable<never>>((e) => throwError(() => toAppError(e)));
  }
}
