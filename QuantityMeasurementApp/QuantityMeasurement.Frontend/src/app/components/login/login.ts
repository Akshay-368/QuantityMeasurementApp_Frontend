import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
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
  isLoggingIn = false;
  isRegistering = false;

  private readonly usernamePattern = /^[A-Za-z0-9_]+$/;
  private readonly hasLetterPattern = /[A-Za-z]/;
  private readonly hasNumberPattern = /\d/;
  private readonly hasSpecialPattern = /[^A-Za-z0-9]/;

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
    if (this.isLoggingIn || this.isRegistering) {
      return;
    }

    const trimmedUsername = this.username.trim();
    const trimmedPassword = this.password.trim();

    if (!trimmedUsername || !trimmedPassword) {
      this.message = 'Username and password are required.';
      return;
    }

    if (!this.usernamePattern.test(trimmedUsername)) {
      this.message = 'Username can contain only letters, numbers, and underscore. No spaces are allowed.';
      return;
    }

    this.isLoggingIn = true;
    this.message = 'Signing in...';

    this.auth
      .login(trimmedUsername, trimmedPassword)
      .pipe(
        finalize(() => {
          this.isLoggingIn = false;
        })
      )
      .subscribe({
      next: (res: string) => {
        const token = this.auth.extractToken(res);
        if (!token) {
          this.message = 'Token missing in login response.';
          return;
        }

        this.auth.saveToken(token, trimmedUsername);
        this.router.navigate(['home']);
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  register(): void {
    if (this.isLoggingIn || this.isRegistering) {
      return;
    }

    const usernameValidation = this.validateRegistrationUsername(this.regUsername);
    if (usernameValidation) {
      this.message = usernameValidation;
      return;
    }

    const passwordValidation = this.validateRegistrationPassword(this.regPassword);
    if (passwordValidation) {
      this.message = passwordValidation;
      return;
    }

    const trimmedUsername = this.regUsername.trim();
    const password = this.regPassword;

    this.isRegistering = true;
    this.message = 'Registering user...';

    this.auth
      .register(trimmedUsername, password)
      .pipe(
        finalize(() => {
          this.isRegistering = false;
        })
      )
      .subscribe({
      next: () => {
        this.message = 'Account created. Please login now.';
        this.regPassword = '';
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  private validateRegistrationUsername(rawUsername: string): string {
    const username = rawUsername.trim();

    if (!username) {
      return 'Username is required for registration.';
    }

    if (!this.usernamePattern.test(username)) {
      return 'Username can contain only letters, numbers, and underscore. No spaces are allowed.';
    }

    return '';
  }

  private validateRegistrationPassword(password: string): string {
    if (!password) {
      return 'Password is required for registration.';
    }

    if (password.length < 8) {
      return 'Password must be at least 8 characters long.';
    }

    if (/\s/.test(password)) {
      return 'Password cannot contain whitespace.';
    }

    if (!this.hasLetterPattern.test(password)) {
      return 'Password must include at least one letter.';
    }

    if (!this.hasNumberPattern.test(password)) {
      return 'Password must include at least one number.';
    }

    if (!this.hasSpecialPattern.test(password)) {
      return 'Password must include at least one special symbol.';
    }

    return '';
  }
}
