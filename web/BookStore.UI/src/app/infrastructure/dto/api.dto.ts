export interface BookDto {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publishedYear: number | null;
  coverUrl: string | null;
  openLibraryKey: string;
  description: string | null;
  genre: string;
  price: number;
  stock: number;
}

export interface PagedResultDto<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface GenreCountDto {
  genre: string;
  count: number;
}

export interface CatalogBookDto {
  title: string;
  author: string;
  isbn: string | null;
  publishedYear: number | null;
  coverUrl: string | null;
  openLibraryKey: string;
}
