import { HttpErrorResponse } from '@angular/common/http';
import { toAppError } from './to-app-error';

describe('toAppError', () => {
  const http = (status: number, error: unknown = null) => new HttpErrorResponse({ status, error });

  it.each([
    [404, 'notFound'],
    [409, 'conflict'],
    [503, 'unavailable'],
    [504, 'timeout'],
    [500, 'unknown'],
    [0, 'unavailable']
  ])('maps status %i to %s', (status, kind) => {
    expect(toAppError(http(status)).kind).toBe(kind);
  });

  it('keeps field errors and detail for validation problems', () => {
    const e = toAppError(http(400, { title: 'Validation', errors: { Price: ['Must be positive'] } }));
    expect(e.kind).toBe('validation');
    expect(e.message).toBe('Validation');
    expect(e.fieldErrors['Price']).toEqual(['Must be positive']);
  });

  it('prefers detail over title', () => {
    expect(toAppError(http(409, { title: 'Conflict', detail: 'ISBN exists' })).message).toBe('ISBN exists');
  });

  it('handles non-HTTP errors', () => {
    expect(toAppError(new Error('x')).kind).toBe('unknown');
  });
});
