export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  statusCode: number;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
