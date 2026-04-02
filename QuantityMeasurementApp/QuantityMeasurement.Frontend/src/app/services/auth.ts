import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // private apiBase = '/api'; as this is causing https://quantity-frontend.onrender.com/api/... when ssr proxy layer is not hit and is served by expres or sdn a static site
  private apiBase = 'https://quantity-backend.onrender.com/api';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

// auth.ts — login()
login(username: string, password: string): Observable<string> {
  return this.http.post(
    `${this.apiBase}/auth/login`,
    { username, password },   // ← send as JSON body
    { responseType: 'text', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) }
  );
}

// Same for register()
register(username: string, password: string): Observable<string> {
  return this.http.post(
    `${this.apiBase}/auth/register`,
    { username, password },   // ← send as JSON body
    { responseType: 'text', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) }
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
      const cleaned = response.replace(/^"|"$/g, '');
      
      // Validate that it looks like a JWT (starts with eyJ and has 3 parts)
      if (this.isValidJwt(cleaned)) {
        return cleaned;
      }
      
      // If it's HTML or doesn't look like a JWT, return empty
      if (cleaned.includes('<') || cleaned.includes('<!')) {
        return '';
      }
      
      return cleaned;
    }

    if (typeof response === 'object' && response !== null && 'token' in response) {
      const token = (response as { token?: string }).token;
      if (token && this.isValidJwt(token)) {
        return token;
      }
      return '';
    }

    return '';
  }

  private isValidJwt(token: string): boolean {
    if (!token) return false;
    if (token.startsWith('<') || token.startsWith('<!')) return false;
    
    const parts = token.split('.');
    return parts.length === 3 && parts.every(part => part.length > 0);
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }

  extractErrorMessage(error: unknown): string {
    const maybeError = error as any;

    // Try to get the error from various possible structures
    const errorBody = maybeError?.error || maybeError?.body || maybeError;

    // If it's a string, return it directly
    if (typeof errorBody === 'string') {
      return errorBody;
    }

    // Try field-level validation errors first
    if (errorBody?.errors && typeof errorBody.errors === 'object') {
      const firstErrorGroup = Object.values(errorBody.errors).find(
        (messages: any) => Array.isArray(messages) && messages.length > 0
      );

      if (firstErrorGroup && Array.isArray(firstErrorGroup) && firstErrorGroup.length > 0) {
        return firstErrorGroup[0];
      }
    }

    // Try detail, title, message fields
    if (errorBody?.detail && typeof errorBody.detail === 'string') {
      return errorBody.detail;
    }

    if (errorBody?.title && typeof errorBody.title === 'string') {
      return errorBody.title;
    }

    if (errorBody?.message && typeof errorBody.message === 'string') {
      return errorBody.message;
    }

    // If it's an object with an error property (double-wrapped)
    if (errorBody?.error && typeof errorBody.error === 'string') {
      return errorBody.error;
    }

    // Fallback: return JSON if it looks structured
    if (typeof errorBody === 'object' && errorBody !== null) {
      try {
        return JSON.stringify(errorBody);
      } catch {
        return 'Request failed';
      }
    }

    return 'Request failed';
  }

  private hasStorage(): boolean {
    return typeof localStorage !== 'undefined';
  }
}
