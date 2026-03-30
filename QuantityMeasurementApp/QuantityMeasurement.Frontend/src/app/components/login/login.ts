import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit {
  username = '';
  password = '';
  regUsername = '';
  regPassword = '';
  message = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.router.navigate(['home']);
    }
  }

  login(): void {
    if (!this.username.trim() || !this.password.trim()) {
      this.message = 'Username and password are required.';
      return;
    }

    this.message = 'Signing in...';

    this.auth.login(this.username.trim(), this.password.trim()).subscribe({
      next: (res: string) => {
        const token = this.auth.extractToken(res);
        if (!token) {
          this.message = 'Token missing in login response.';
          return;
        }

        this.auth.saveToken(token, this.username.trim());
        this.router.navigate(['home']);
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  register(): void {
    if (!this.regUsername.trim() || !this.regPassword.trim()) {
      this.message = 'Username and password are required.';
      return;
    }

    this.message = 'Creating account...';

    this.auth.register(this.regUsername.trim(), this.regPassword.trim()).subscribe({
      next: () => {
        this.message = 'Account created. Please login now.';
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }
}
