import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authBaseUrl = environment.authApiBaseUrl;

  constructor(private httpClient: HttpClient) {}

  login(email: string, password: string) {
    return this.httpClient.post(`${this.authBaseUrl}/login`, {
      email,
      password
    });
  }

  register(email: string, password: string) {
    return this.httpClient.post(`${this.authBaseUrl}/register`, {
      email,
      password
    });
  }
}
