import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { CatalogBook } from '../../core/models/catalog-book';
import { CATALOG_REPOSITORY } from '../tokens/repository.tokens';

@Injectable({ providedIn: 'root' })
export class CatalogSearch {
  private readonly repo = inject(CATALOG_REPOSITORY);

  search(term: string, limit = 10): Observable<readonly CatalogBook[]> {
    const query = term.trim();
    return query ? this.repo.search(query, limit) : of([]);
  }
}
