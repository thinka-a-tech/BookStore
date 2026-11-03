import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ButtonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly http = inject(HttpClient);
  protected readonly status = signal('Click the button to call /api/books/genres through the proxy.');

  protected check(): void {
    this.http.get<unknown[]>('/api/books/genres').subscribe({
      next: genres => this.status.set('API reachable: ' + genres.length + ' genres'),
      error: err => this.status.set('API call failed: ' + err.status)
    });
  }
}
