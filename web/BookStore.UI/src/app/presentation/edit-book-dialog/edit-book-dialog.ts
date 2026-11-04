import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputNumber } from 'primeng/inputnumber';
import { Message } from 'primeng/message';
import { Select } from 'primeng/select';
import { Textarea } from 'primeng/textarea';
import { BookCommands } from '../../application/books/book-commands';
import { AppError } from '../../core/errors/app-error';
import { Book } from '../../core/models/book';
import { Genre, GENRES } from '../../core/models/genre';

/** Only the store-owned fields are editable; the Open Library fields are read-only. */
@Component({
  selector: 'app-edit-book-dialog',
  imports: [FormsModule, Dialog, Button, InputNumber, Select, Textarea, Message],
  templateUrl: './edit-book-dialog.html'
})
export class EditBookDialog implements OnInit {
  private readonly commands = inject(BookCommands);
  private readonly messages = inject(MessageService);

  readonly book = input.required<Book>();
  readonly closed = output<void>();
  protected readonly genres = [...GENRES];

  protected readonly genre = signal<Genre | null>(null);
  protected readonly price = signal<number | null>(null);
  protected readonly stock = signal(0);
  protected readonly description = signal('');
  protected readonly saving = signal(false);
  protected readonly error = signal<AppError | null>(null);

  ngOnInit(): void {
    const book = this.book();
    this.genre.set(book.genre);
    this.price.set(book.price);
    this.stock.set(book.stock);
    this.description.set(book.description ?? '');
  }

  protected canSave(): boolean {
    return !!this.genre() && (this.price() ?? 0) > 0 && !this.saving();
  }

  protected fieldMessages(error: AppError): string[] {
    return Object.values(error.fieldErrors).flat();
  }

  protected save(): void {
    const genre = this.genre();
    const price = this.price();
    if (!genre || !price) {
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    this.commands
      .update(this.book().id, { genre, price, stock: this.stock(), description: this.description().trim() || null })
      .subscribe({
        next: () => {
          this.messages.add({ severity: 'success', summary: 'Book updated', detail: this.book().title });
          this.closed.emit();
        },
        error: (e: AppError) => {
          this.saving.set(false);
          this.error.set(e);
        }
      });
  }
}
