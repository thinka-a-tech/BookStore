import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { Message } from 'primeng/message';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { BookCommands } from '../../application/books/book-commands';
import { BooksStore } from '../../application/books/books.store';
import { Book } from '../../core/models/book';
import { BookSortField } from '../../core/models/book-requests';
import { AddBookDialog } from '../add-book-dialog/add-book-dialog';
import { EditBookDialog } from '../edit-book-dialog/edit-book-dialog';

@Component({
  selector: 'app-body',
  imports: [CurrencyPipe, TableModule, Button, Message, AddBookDialog, EditBookDialog],
  templateUrl: './body.html'
})
export class Body {
  protected readonly store = inject(BooksStore);
  private readonly commands = inject(BookCommands);
  private readonly confirmation = inject(ConfirmationService);
  private readonly messages = inject(MessageService);

  protected readonly adding = signal(false);
  protected readonly editing = signal<Book | null>(null);

  protected readonly books = computed(() => (this.store.result()?.items ?? []) as Book[]);
  protected readonly first = computed(() => (this.store.query().page - 1) * this.store.query().pageSize);
  protected readonly sortField = computed(() => {
    const field = this.store.query().sortBy;
    return field.charAt(0).toLowerCase() + field.slice(1);
  });

  protected onLazyLoad(event: TableLazyLoadEvent): void {
    const query = this.store.query();
    const rows = event.rows ?? query.pageSize;
    const page = Math.floor((event.first ?? 0) / rows) + 1;
    const field =
      typeof event.sortField === 'string'
        ? (event.sortField.charAt(0).toUpperCase() + event.sortField.slice(1) as BookSortField)
        : query.sortBy;
    const descending = event.sortOrder === -1;

    if (field !== query.sortBy || descending !== query.descending) {
      this.store.sort(field, descending);
    } else if (page !== query.page || rows !== query.pageSize) {
      this.store.goToPage(page, rows);
    }
  }

  protected confirmDelete(book: Book): void {
    this.confirmation.confirm({
      header: 'Delete book',
      message: `Delete "${book.title}"? This cannot be undone.`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonProps: { label: 'Delete', severity: 'danger' },
      rejectButtonProps: { label: 'Cancel', severity: 'secondary', outlined: true },
      accept: () =>
        this.commands.remove(book.id).subscribe({
          next: () => this.messages.add({ severity: 'success', summary: 'Deleted', detail: book.title }),
          error: (e) => this.messages.add({ severity: 'error', summary: 'Delete failed', detail: e.message })
        })
    });
  }
}

