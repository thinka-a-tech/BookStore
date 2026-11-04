import { TestBed } from '@angular/core/testing';
import { ConfirmationService, MessageService } from 'primeng/api';
import { of } from 'rxjs';
import { BOOK_REPOSITORY, CATALOG_REPOSITORY } from './application/tokens/repository.tokens';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    const books = {
      list: () => of({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 }),
      genreCounts: () => of([])
    };
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        MessageService,
        ConfirmationService,
        { provide: BOOK_REPOSITORY, useValue: books },
        { provide: CATALOG_REPOSITORY, useValue: { search: () => of([]) } }
      ]
    }).compileComponents();
  });

  it('renders Header, Body and Footer as siblings in the root', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const root = fixture.nativeElement as HTMLElement;
    for (const tag of ['app-header', 'app-body', 'app-footer']) {
      expect(root.querySelector(tag)?.parentElement?.tagName.toLowerCase()).toBe('div');
    }
    expect(root.querySelector('app-header app-body, app-body app-header, app-body app-footer')).toBeNull();
  });

  it('shows the empty state when there are no books', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('No books found.');
  });
});

