import { Component, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select } from 'primeng/select';
import { BooksStore } from '../../application/books/books.store';
import { GenreFilterStore } from '../../application/state/genre-filter.store';
import { Genre, GENRES } from '../../core/models/genre';

@Component({
  selector: 'app-header',
  imports: [FormsModule, Select],
  templateUrl: './header.html'
})
export class Header {
  protected readonly filter = inject(GenreFilterStore);
  private readonly store = inject(BooksStore);
  private readonly counts = this.store.genreCounts;
  private readonly total = this.store.totalBooks;

  protected readonly options = computed<{ label: string; value: Genre | null }[]>(() => [
    { label: `All genres (${this.total()})`, value: null },
    ...GENRES.map((genre) => {
      const count = this.counts().find((c) => c.genre === genre)?.count ?? 0;
      return { label: `${genre} (${count})`, value: genre };
    })
  ]);
}
