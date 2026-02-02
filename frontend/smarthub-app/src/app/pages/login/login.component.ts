import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  error = '';

  private readonly PASSWORD_REGEX =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        RxwebValidators.required({ message: 'Email is required' }),
        RxwebValidators.email({ message: 'Invalid email format' }),
      ]),
      password: new FormControl('', [
        RxwebValidators.required({ message: 'Password is required' }),
        RxwebValidators.pattern({
          expression: { password: this.PASSWORD_REGEX },
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
