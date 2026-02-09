import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  RxwebValidators,
  RxReactiveFormsModule,
} from '@rxweb/reactive-form-validators';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RxReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup({
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

  register(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const { email, password } = this.registerForm.value;

    this.authService.register(email, password).subscribe({
      next: () => this.router.navigate(['/auth/login']),
      error: () => (this.error = 'Registration failed'),
    });
  }
}
