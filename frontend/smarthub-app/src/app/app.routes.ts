import { Routes } from '@angular/router';

import { AuthLayoutComponent } from './core/layout/auth-layout/auth-layout.component';
import { AppLayoutComponent } from './core/layout/app-layout/app-layout.component';

import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';

import { DashboardComponent } from './features/dashboard/dashboard/dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
    ],
  },

  {
    path: '',
    component: AppLayoutComponent,
    children: [{ path: 'dashboard', component: DashboardComponent }],
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: '**', redirectTo: 'login' },
];
