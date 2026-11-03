import { Observable } from 'rxjs';
import { CatalogBook } from '../models/catalog-book';

/** Port for the Open Library catalog, proxied through the API. Failures are emitted as AppError. */
export interface CatalogRepository {
  search(query: string, limit?: number): Observable<readonly CatalogBook[]>;
}
