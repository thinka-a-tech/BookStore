export const GENRES = [
  'Fiction',
  'SciFi',
  'Fantasy',
  'Mystery',
  'Biography',
  'NonFiction',
  'Children',
  'History',
] as const;

export type Genre = (typeof GENRES)[number];
