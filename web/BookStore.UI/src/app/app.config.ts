import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';

import { routes } from './app.routes';
import { BOOK_REPOSITORY, CATALOG_REPOSITORY } from './application/tokens/repository.tokens';
import { HttpBookRepository } from './infrastructure/http/http-book.repository';
import { HttpCatalogRepository } from './infrastructure/http/http-catalog.repository';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    { provide: BOOK_REPOSITORY, useClass: HttpBookRepository },
    { provide: CATALOG_REPOSITORY, useClass: HttpCatalogRepository },
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: false,
          cssLayer: {
            name: 'primeng',
            order: 'theme, base, primeng'
          }
        }
      }
    })
  ]
};

