import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private jwtHelper: JwtHelperService
  ) {}

  canActivate(): boolean {
    const token = sessionStorage.getItem('token');
    const user = sessionStorage.getItem('user');
    
    // Check if both token and user exist
    if (token && user) {
      try {
        // Validate user data can be parsed
        const userData = JSON.parse(user);
        if (userData && userData.userRole) {
          return true;
        }
      } catch (error) {
        // Clear invalid data
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('user');
      }
    }
    
    this.router.navigate(['/auth/login']);
    return false;
  }
}