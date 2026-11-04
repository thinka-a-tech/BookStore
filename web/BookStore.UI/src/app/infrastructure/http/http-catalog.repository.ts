import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { CatalogBook } from '../../core/models/catalog-book';
import { CatalogRepository } from '../../core/repositories/catalog.repository';
import { API_BASE_URL } from '../config/api-base-url';
import { CatalogBookDto } from '../dto/api.dto';
import { toAppError } from '../http/to-app-error';
import { toCatalogBook } from '../mappers/api.mappers';

@Injectable()
export class HttpCatalogRepository implements CatalogRepository {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_BASE_URL)}/catalog/search`;

  search(query: string, limit = 10): Observable<readonly CatalogBook[]> {
    const params = new HttpParams().set('q', query).set('limit', limit);
    return this.http.get<CatalogBookDto[]>(this.url, { params }).pipe(
      map((items) => items.map(toCatalogBook)),
      catchError((e) => throwError(() => toAppError(e)))
    );
  }
}
