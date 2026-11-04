import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputNumber } from 'primeng/inputnumber';
import { InputText } from 'primeng/inputtext';
import { Message } from 'primeng/message';
import { Select } from 'primeng/select';
import { Textarea } from 'primeng/textarea';
import { BookCommands } from '../../application/books/book-commands';
import { CatalogSearch } from '../../application/catalog/catalog-search';
import { AppError } from '../../core/errors/app-error';
import { CatalogBook } from '../../core/models/catalog-book';
import { Genre, GENRES } from '../../core/models/genre';

/** Step 1: search Open Library and pick a result. Step 2: set genre, price and stock, then save. */
@Component({
  selector: 'app-add-book-dialog',
  imports: [FormsModule, Dialog, Button, InputText, InputNumber, Select, Textarea, Message],
  templateUrl: './add-book-dialog.html'
})
export class AddBookDialog {
  private readonly catalog = inject(CatalogSearch);
  private readonly commands = inject(BookCommands);
  private readonly messages = inject(MessageService);

  readonly closed = output<void>();
  protected readonly genres = [...GENRES];

  protected readonly term = signal('');
  protected readonly searching = signal(false);
  protected readonly results = signal<readonly CatalogBook[] | null>(null);
  protected readonly searchError = signal<AppError | null>(null);

  protected readonly selected = signal<CatalogBook | null>(null);
  protected readonly genre = signal<Genre | null>(null);
  protected readonly price = signal<number | null>(null);
  protected readonly stock = signal(0);
  protected readonly description = signal('');
  protected readonly saving = signal(false);
  protected readonly saveError = signal<AppError | null>(null);

  protected search(): void {
    this.searching.set(true);
    this.searchError.set(null);
    this.catalog.search(this.term()).subscribe({
      next: (list) => {
        this.results.set(list);
        this.searching.set(false);
      },
      error: (e: AppError) => {
        this.results.set(null);
        this.searchError.set(e);
        this.searching.set(false);
      }
    });
  }

  protected select(book: CatalogBook): void {
    this.selected.set(book);
    this.saveError.set(null);
  }

  protected canSave(): boolean {
    return !!this.selected()?.isbn && !!this.genre() && (this.price() ?? 0) > 0 && !this.saving();
  }

  protected fieldMessages(error: AppError): string[] {
    return Object.values(error.fieldErrors).flat();
  }

  protected save(): void {
    const book = this.selected();
    const genre = this.genre();
    const price = this.price();
    if (!book?.isbn || !genre || !price) {
      return;
    }
    this.saving.set(true);
    this.saveError.set(null);
    this.commands
      .add({
        title: book.title,
        author: book.author,
        isbn: book.isbn,
        openLibraryKey: book.openLibraryKey,
        publishedYear: book.publishedYear,
        coverUrl: book.coverUrl,
        genre,
        price,
        stock: this.stock(),
        description: this.description().trim() || null
      })
      .subscribe({
        next: () => {
          this.messages.add({ severity: 'success', summary: 'Book added', detail: book.title });
          this.closed.emit();
        },
        error: (e: AppError) => {
          this.saving.set(false);
          this.saveError.set(e);
        }
      });
  }
}
