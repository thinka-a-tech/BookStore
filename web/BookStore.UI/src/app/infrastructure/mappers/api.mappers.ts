import { Book } from '../../core/models/book';
import { CatalogBook } from '../../core/models/catalog-book';
import { Genre } from '../../core/models/genre';
import { GenreCount } from '../../core/models/genre-count';
import { PagedResult } from '../../core/models/paged-result';
import { BookDto, CatalogBookDto, GenreCountDto, PagedResultDto } from '../dto/api.dto';

export const toBook = (dto: BookDto): Book => ({ ...dto, genre: dto.genre as Genre });

export const toGenreCount = (dto: GenreCountDto): GenreCount => ({ genre: dto.genre as Genre, count: dto.count });

export const toCatalogBook = (dto: CatalogBookDto): CatalogBook => ({ ...dto });

export const toPagedBooks = (dto: PagedResultDto<BookDto>): PagedResult<Book> => ({
  items: dto.items.map(toBook),
  page: dto.page,
  pageSize: dto.pageSize,
  totalCount: dto.totalCount,
  totalPages: dto.totalPages
});
