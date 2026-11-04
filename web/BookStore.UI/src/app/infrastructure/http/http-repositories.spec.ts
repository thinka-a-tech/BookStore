import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppError } from '../../core/errors/app-error';
import { HttpBookRepository } from './http-book.repository';
import { HttpCatalogRepository } from './http-catalog.repository';

describe('HTTP repositories', () => {
  let ctrl: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), HttpBookRepository, HttpCatalogRepository]
    });
    ctrl = TestBed.inject(HttpTestingController);
  });

  afterEach(() => ctrl.verify());

  it('lists books with query params and maps the page', () => {
    let total = -1;
    TestBed.inject(HttpBookRepository)
      .list({ page: 2, pageSize: 5, sortBy: 'Price', descending: true, genre: 'Fantasy' })
      .subscribe((r) => (total = r.totalCount));

    const req = ctrl.expectOne((r) => r.url === '/api/books');
    expect(req.request.params.get('page')).toBe('2');
    expect(req.request.params.get('sortBy')).toBe('Price');
    expect(req.request.params.get('descending')).toBe('true');
    expect(req.request.params.get('genre')).toBe('Fantasy');
    req.flush({ items: [], page: 2, pageSize: 5, totalCount: 7, totalPages: 2 });
    expect(total).toBe(7);
  });

  it('omits genre when not filtering', () => {
    TestBed.inject(HttpBookRepository)
      .list({ page: 1, pageSize: 10, sortBy: 'Title', descending: false, genre: null })
      .subscribe();
    const req = ctrl.expectOne((r) => r.url === '/api/books');
    expect(req.request.params.has('genre')).toBe(false);
    req.flush({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 });
  });

  it('emits AppError on 409', () => {
    let error: AppError | undefined;
    TestBed.inject(HttpBookRepository)
      .delete(1)
      .subscribe({ error: (e) => (error = e) });
    ctrl.expectOne('/api/books/1').flush({ detail: 'nope' }, { status: 409, statusText: 'Conflict' });
    expect(error?.kind).toBe('conflict');
    expect(error?.message).toBe('nope');
  });

  it('searches the catalog', () => {
    let count = 0;
    TestBed.inject(HttpCatalogRepository)
      .search('dune', 3)
      .subscribe((r) => (count = r.length));
    const req = ctrl.expectOne((r) => r.url === '/api/catalog/search');
    expect(req.request.params.get('q')).toBe('dune');
    expect(req.request.params.get('limit')).toBe('3');
    req.flush([{ title: 't', author: 'a', isbn: null, publishedYear: null, coverUrl: null, openLibraryKey: 'k' }]);
    expect(count).toBe(1);
  });
});
