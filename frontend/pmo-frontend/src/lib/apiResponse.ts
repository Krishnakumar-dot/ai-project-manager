export interface ApiResponse<T> {
  isSuccessful: boolean;
  message: string;
  response: T;
}
