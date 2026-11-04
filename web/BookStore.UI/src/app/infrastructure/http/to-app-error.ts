import { HttpErrorResponse } from '@angular/common/http';
import { AppError, AppErrorKind } from '../../core/errors/app-error';

const kindByStatus: Record<number, AppErrorKind> = {
  400: 'validation',
  404: 'notFound',
  409: 'conflict',
  503: 'unavailable',
  504: 'timeout'
};

const fallbackMessage: Record<AppErrorKind, string> = {
  validation: 'The request was invalid.',
  notFound: 'The requested item was not found.',
  conflict: 'The item conflicts with an existing one.',
  unavailable: 'The service is temporarily unavailable.',
  timeout: 'The service took too long to respond.',
  unknown: 'Something went wrong.'
};

/** Normalizes an HTTP failure (ProblemDetails from the API) into an AppError. */
export function toAppError(error: unknown): AppError {
  if (!(error instanceof HttpErrorResponse)) {
    return { kind: 'unknown', message: fallbackMessage.unknown, fieldErrors: {} };
  }

  // status 0 means the API could not be reached at all
  const kind: AppErrorKind = error.status === 0 ? 'unavailable' : (kindByStatus[error.status] ?? 'unknown');
  const body = error.error as { detail?: string; title?: string; errors?: Record<string, string[]> } | null;

  return {
    kind,
    message: body?.detail || body?.title || fallbackMessage[kind],
    fieldErrors: kind === 'validation' && body?.errors ? body.errors : {}
  };
}
