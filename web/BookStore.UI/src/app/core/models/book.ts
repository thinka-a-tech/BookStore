import { Genre } from './genre';

export interface Book {
  readonly id: number;
  readonly title: string;
  readonly author: string;
  readonly isbn: string;
  readonly openLibraryKey: string;
  readonly genre: Genre;
  readonly price: number;
  readonly stock: number;
  readonly publishedYear: number | null;
  readonly coverUrl: string | null;
  readonly description: string | null;
}
