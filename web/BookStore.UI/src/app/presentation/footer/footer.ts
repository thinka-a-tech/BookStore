import { Component, inject } from '@angular/core';
import { BooksStore } from '../../application/books/books.store';
import { GenreFilterStore } from '../../application/state/genre-filter.store';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.html'
})
export class Footer {
  protected readonly store = inject(BooksStore);
  protected readonly filter = inject(GenreFilterStore);
}
