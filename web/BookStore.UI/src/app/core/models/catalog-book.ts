/** A transient Open Library search result */
export interface CatalogBook {
  readonly title: string;
  readonly author: string;
  readonly isbn: string | null;
  readonly publishedYear: number | null;
  readonly coverUrl: string | null;
  readonly openLibraryKey: string;
}
