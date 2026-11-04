import { TestBed } from '@angular/core/testing';
import { Observable, of, throwError } from 'rxjs';
import { AppError } from '../../core/errors/app-error';
import { Book } from '../../core/models/book';
import { BookListQuery } from '../../core/models/book-requests';
import { PagedResult } from '../../core/models/paged-result';
import { BookRepository } from '../../core/repositories/book.repository';
import { GenreFilterStore } from '../state/genre-filter.store';
import { BOOK_REPOSITORY } from '../tokens/repository.tokens';
import { BookCommands } from './book-commands';
import { BooksStore } from './books.store';

const page = (p: number, items: Book[], totalPages = 1): PagedResult<Book> => ({ items, page: p, pageSize: 10, totalCount: items.length, totalPages });
const book = { id: 1, title: 'T' } as Book;

describe('BooksStore', () => {
  let queries: BookListQuery[];
  let listImpl: (q: BookListQuery) => Observable<PagedResult<Book>>;
  let repo: BookRepository;

  beforeEach(() => {
    queries = [];
    listImpl = (q) => of(page(q.page, [book]));
    repo = {
      list: (q) => {
        queries.push(q);
        return listImpl(q);
      },
      genreCounts: () => of([{ genre: 'Fantasy', count: 2 }, { genre: 'History', count: 3 }]),
      get: () => of(book),
      create: () => of(1),
      update: () => of(book),
      delete: () => of(undefined)
    };
    TestBed.configureTestingModule({ providers: [{ provide: BOOK_REPOSITORY, useValue: repo }] });
  });

  it('loads the first page and genre counts', () => {
    const store = TestBed.inject(BooksStore);
    TestBed.tick();
    expect(store.result()?.items).toHaveLength(1);
    expect(store.totalBooks()).toBe(5);
    expect(store.loading()).toBe(false);
  });

  it('resets to page 1 when the genre filter changes', () => {
    const store = TestBed.inject(BooksStore);
    TestBed.tick();
    store.goToPage(3);
    TestBed.tick();
    TestBed.inject(GenreFilterStore).select('Fantasy');
    TestBed.tick();
    expect(queries.at(-1)).toMatchObject({ page: 1, genre: 'Fantasy' });
  });

  it('exposes the error when loading fails', () => {
    const error: AppError = { kind: 'unavailable', message: 'down', fieldErrors: {} };
    listImpl = () => throwError(() => error);
    const store = TestBed.inject(BooksStore);
    TestBed.tick();
    expect(store.error()).toEqual(error);
  });

  it('steps back a page when the current page becomes empty', () => {
    listImpl = (q) => of(q.page === 2 ? page(2, [], 1) : page(1, [book], 1));
    const store = TestBed.inject(BooksStore);
    TestBed.tick();
    store.goToPage(2);
    TestBed.tick();
    TestBed.tick();
    expect(queries.at(-1)?.page).toBe(1);
  });

  it('reloads after a successful delete', () => {
    TestBed.inject(BooksStore);
    TestBed.tick();
    const before = queries.length;
    TestBed.inject(BookCommands).remove(1).subscribe();
    TestBed.tick();
    expect(queries.length).toBe(before + 1);
  });
});

