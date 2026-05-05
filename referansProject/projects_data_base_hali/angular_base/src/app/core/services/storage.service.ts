import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  setItem(key: string, value: string): void {
    try {
      localStorage.setItem(key, value);
    } catch (e) {
      console.error('StorageService: setItem error', e);
    }
  }

  getItem(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch (e) {
      console.error('StorageService: getItem error', e);
      return null;
    }
  }

  removeItem(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch (e) {
      console.error('StorageService: removeItem error', e);
    }
  }

  clear(): void {
    try {
      localStorage.clear();
    } catch (e) {
      console.error('StorageService: clear error', e);
    }
  }

  setObject<T>(key: string, value: T): void {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (e) {
      console.error('StorageService: setObject error', e);
    }
  }

  getObject<T>(key: string): T | null {
    try {
      const item = localStorage.getItem(key);
      if (item) {
        return JSON.parse(item) as T;
      }
      return null;
    } catch (e) {
      console.error('StorageService: getObject error', e);
      return null;
    }
  }

  hasItem(key: string): boolean {
    return localStorage.getItem(key) !== null;
  }
}
