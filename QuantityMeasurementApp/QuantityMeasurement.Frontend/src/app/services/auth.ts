import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiBase = '/api';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(username: string, password: string): Observable<string> {
    return this.http.post(
      `${this.apiBase}/auth/login?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`,
      {},
      { responseType: 'text' }
    );
  }

  register(username: string, password: string): Observable<string> {
    return this.http.post(
      `${this.apiBase}/auth/register?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`,
      {},
      { responseType: 'text' }
    );
  }

  saveToken(token: string, username: string): void {
    if (!this.hasStorage()) {
      return;
    }

    localStorage.setItem('qm_token', token);
    localStorage.setItem('qm_username', username);
  }

  getToken(): string | null {
    if (!this.hasStorage()) {
      return null;
    }

    return localStorage.getItem('qm_token');
  }

  isLoggedIn(): boolean {
    if (!this.hasStorage()) {
      return false;
    }

    return !!localStorage.getItem('qm_token');
  }

  logout(): void {
    if (!this.hasStorage()) {
      this.router.navigate(['']);
      return;
    }

    localStorage.removeItem('qm_token');
    localStorage.removeItem('qm_username');
    this.router.navigate(['']);
  }

  extractToken(response: unknown): string {
    if (typeof response === 'string') {
      return response.replace(/^"|"$/g, '');
    }

    if (typeof response === 'object' && response !== null && 'token' in response) {
      const token = (response as { token?: string }).token;
      return token ?? '';
    }

    return '';
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }

  extractErrorMessage(error: unknown): string {
    const maybeError = error as { error?: { detail?: string; title?: string } };

    if (maybeError?.error?.detail) {
      return maybeError.error.detail;
    }

    if (maybeError?.error?.title) {
      return maybeError.error.title;
    }

    return 'Request failed';
  }

  private hasStorage(): boolean {
    return typeof localStorage !== 'undefined';
  }
}
