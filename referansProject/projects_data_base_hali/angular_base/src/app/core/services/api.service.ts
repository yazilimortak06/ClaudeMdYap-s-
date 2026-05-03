import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ApiResponse, PagedResult } from '../models/api-response.model';
import { environment } from '../../../environments/environment';

export interface QueryParams {
  [key: string]: string | number | boolean | null | undefined;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  private buildParams(params?: QueryParams): HttpParams {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach((key) => {
        const value = params[key];
        if (value !== null && value !== undefined) {
          httpParams = httpParams.set(key, String(value));
        }
      });
    }
    return httpParams;
  }

  get<T>(endpoint: string, params?: QueryParams): Observable<ApiResponse<T>> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, {
      params: this.buildParams(params)
    });
  }

  getPaged<T>(
    endpoint: string,
    params?: QueryParams
  ): Observable<ApiResponse<PagedResult<T>>> {
    return this.http.get<ApiResponse<PagedResult<T>>>(
      `${this.baseUrl}/${endpoint}`,
      { params: this.buildParams(params) }
    );
  }

  post<T>(endpoint: string, body: unknown): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(
      `${this.baseUrl}/${endpoint}`,
      body
    );
  }

  put<T>(
    endpoint: string,
    body: unknown
  ): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(
      `${this.baseUrl}/${endpoint}`,
      body
    );
  }

  patch<T>(
    endpoint: string,
    body: unknown
  ): Observable<ApiResponse<T>> {
    return this.http.patch<ApiResponse<T>>(
      `${this.baseUrl}/${endpoint}`,
      body
    );
  }

  delete<T>(endpoint: string): Observable<ApiResponse<T>> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`);
  }

  postFormData<T>(
    endpoint: string,
    formData: FormData
  ): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(
      `${this.baseUrl}/${endpoint}`,
      formData
    );
  }
}
