import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API_URL = 'http://localhost:5227/api/Auth';
  private readonly TOKEN_KEY = 'nzola_token';
  private readonly USER_KEY = 'nzola_user';

  currentUser = signal<User | null>(null);
  private _isLoggedIn = computed(() => this.currentUser() !== null);
  private _isAdmin = computed(() => this.currentUser()?.role === 'admin');

  constructor(private http: HttpClient, private router: Router) {
    this.loadUser();
  }

  private loadUser(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const user = localStorage.getItem(this.USER_KEY);
    if (token && user) {
      this.currentUser.set(JSON.parse(user));
    }
  }

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/login`, dto).pipe(
      tap(res => this.handleAuth(res))
    );
  }

  register(dto: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/register`, dto).pipe(
      tap(res => this.handleAuth(res))
    );
  }

  private handleAuth(res: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, res.token);
    const user: User = {
      id: res.id,
      nome: res.nome,
      email: res.email,
      fotoPerfil: res.fotoPerfil,
      role: res.role,
      privacidade: 'publico'
    };
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return this._isLoggedIn();
  }

  isAdmin(): boolean {
    return this._isAdmin();
  }

  updateCurrentUser(data: Partial<User>): void {
    const current = this.currentUser();
    if (!current) return;
    const updated = { ...current, ...data };
    localStorage.setItem(this.USER_KEY, JSON.stringify(updated));
    this.currentUser.set(updated);
  }
}
