import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import {
  RxReactiveFormsModule,
  RxwebValidators,
} from '@rxweb/reactive-form-validators';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RxReactiveFormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        RxwebValidators.required({ message: 'Email is required' }),
        RxwebValidators.email({
          message: 'Please enter a valid email address',
        }),
      ]),
      password: new FormControl('', [
        RxwebValidators.required({ message: 'Password is required' }),
        RxwebValidators.password({
          validation: {
            minLength: 8,
            digit: true,
            specialCharacter: true,
            upperCase: true,
            lowerCase: true,
          },
          message:
            'Password must be at least 8 characters and include uppercase, lowercase, number and special character',
        }),
      ]),
    });
  }

  login(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.error = '';

    const email = this.loginForm.controls.email!.value;
    const password = this.loginForm.controls.password!.value;

    this.authService.login(email, password).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: () => {
        this.error = 'Invalid email or password';
      },
    });
  }
}
