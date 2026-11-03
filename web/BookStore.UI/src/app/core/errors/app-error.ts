export type AppErrorKind =
  | 'validation'
  | 'notFound'
  | 'conflict'
  | 'unavailable'
  | 'timeout'
  | 'unknown';

export interface AppError {
  readonly kind: AppErrorKind;
  readonly message: string;
  readonly fieldErrors: Readonly<Record<string, readonly string[]>>;
}

export function isAppError(value: unknown): value is AppError {
  return typeof value === 'object' && value !== null && 'kind' in value && 'message' in value && 'fieldErrors' in value;
}
