import { InjectionToken } from '@angular/core';
import { BookRepository } from '../../core/repositories/book.repository';
import { CatalogRepository } from '../../core/repositories/catalog.repository';

export const BOOK_REPOSITORY = new InjectionToken<BookRepository>('BOOK_REPOSITORY');
export const CATALOG_REPOSITORY = new InjectionToken<CatalogRepository>('CATALOG_REPOSITORY');
