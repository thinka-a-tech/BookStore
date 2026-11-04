import { Injectable, computed, inject, linkedSignal, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { catchError, map, of, switchMap, tap } from 'rxjs';
import { AppError } from '../../core/errors/app-error';
import { Book } from '../../core/models/book';
import { BookSortField } from '../../core/models/book-requests';
import { Genre } from '../../core/models/genre';
import { GenreCount } from '../../core/models/genre-count';
import { PagedResult } from '../../core/models/paged-result';
import { GenreFilterStore } from '../state/genre-filter.store';
import { BOOK_REPOSITORY } from '../tokens/repository.tokens';

/** Holds the book grid state (paging, sorting, genre filter) and the genre counts. */
@Injectable({ providedIn: 'root' })
export class BooksStore {
  private readonly repo = inject(BOOK_REPOSITORY);
  private readonly filter = inject(GenreFilterStore);

  // linkedSignal drops back to page 1 whenever the genre filter changes, including when it is cleared
  private readonly paging = linkedSignal<Genre | null, { page: number; pageSize: number; sortBy: BookSortField; descending: boolean }>({
    source: this.filter.genre,
    computation: (_genre, previous) => ({ ...(previous?.value ?? { pageSize: 10, sortBy: 'Title', descending: false }), page: 1 })
  });
  private readonly refreshTick = signal(0);

  readonly result = signal<PagedResult<Book> | null>(null);
  readonly loading = signal(false);
  readonly error = signal<AppError | null>(null);
  readonly genreCounts = signal<readonly GenreCount[]>([]);

  readonly totalBooks = computed(() => this.genreCounts().reduce((sum, g) => sum + g.count, 0));
  readonly query = computed(() => ({ ...this.paging(), genre: this.filter.genre() }));

  constructor() {
    const request = computed(() => ({ query: this.query(), tick: this.refreshTick() }));

    toObservable(request)
      .pipe(
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap(({ query }) =>
          this.repo.list(query).pipe(
            map((result) => ({ result, error: null as AppError | null })),
            catchError((error: AppError) => of({ result: null, error }))
          )
        ),
        takeUntilDestroyed()
      )
      .subscribe(({ result, error }) => {
        this.loading.set(false);
        if (error) {
          this.error.set(error);
          return;
        }
        // the last item of the last page was removed: step back to a page that exists
        if (result && result.items.length === 0 && result.page > 1 && result.totalPages > 0) {
          this.goToPage(result.totalPages);
          return;
        }
        this.result.set(result);
      });

    toObservable(this.refreshTick)
      .pipe(
        switchMap(() => this.repo.genreCounts().pipe(catchError(() => of(this.genreCounts())))),
        takeUntilDestroyed()
      )
      .subscribe((counts) => this.genreCounts.set(counts));
  }

  goToPage(page: number, pageSize = this.query().pageSize): void {
    const q = this.query();
    this.paging.set({ pageSize, sortBy: q.sortBy, descending: q.descending, page });
  }

  sort(sortBy: BookSortField, descending: boolean): void {
    const q = this.query();
    this.paging.set({ pageSize: q.pageSize, sortBy, descending, page: 1 });
  }

  reload(): void {
    this.refreshTick.update((n) => n + 1);
  }
}


