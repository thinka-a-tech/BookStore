import { Injectable, signal } from '@angular/core';
import { Genre } from '../../core/models/genre';

/** Shared by the sibling Header (writes) and Body/Footer (read); null means all genres. */
@Injectable({ providedIn: 'root' })
export class GenreFilterStore {
  private readonly selected = signal<Genre | null>(null);
  readonly genre = this.selected.asReadonly();

  select(genre: Genre | null): void {
    this.selected.set(genre);
  }
}
