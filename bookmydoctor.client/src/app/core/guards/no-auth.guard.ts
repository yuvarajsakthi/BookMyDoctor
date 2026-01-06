import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = sessionStorage.getItem('token');
    const user = sessionStorage.getItem('user');
    
    // If user is authenticated, redirect to dashboard
    if (token && user) {
      try {
        const userData = JSON.parse(user);
        if (userData && userData.userRole) {
          const role = userData.userRole.toLowerCase();
          this.router.navigate([`/dashboard/${role}`]);
          return false;
        }
      } catch (error) {
        // Invalid user data, allow access to auth routes
        return true;
      }
    }
    
    // Not authenticated, allow access to auth routes
    return true;
  }
}