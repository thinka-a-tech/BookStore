import { Genre } from './genre';

export type BookSortField = 'Title' | 'Author' | 'Genre' | 'Price' | 'PublishedYear' | 'Stock';

export interface BookListQuery {
  readonly page: number;
  readonly pageSize: number;
  readonly sortBy: BookSortField;
  readonly descending: boolean;
  readonly genre: Genre | null;
}

/** Input for adding a book: the selected Open Library fields plus the store-owned fields. */
export interface NewBook {
  readonly title: string;
  readonly author: string;
  readonly isbn: string;
  readonly openLibraryKey: string;
  readonly publishedYear: number | null;
  readonly coverUrl: string | null;
  readonly genre: Genre;
  readonly price: number;
  readonly stock: number;
  readonly description: string | null;
}

/** Only the store-owned fields can change after a book is added. */
export interface BookChanges {
  readonly genre: Genre;
  readonly price: number;
  readonly stock: number;
  readonly description: string | null;
}
