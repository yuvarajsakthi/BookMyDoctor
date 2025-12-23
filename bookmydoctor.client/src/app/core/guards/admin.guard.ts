import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private router: Router,
    private jwtHelper: JwtHelperService
  ) {}

  canActivate(): boolean {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (user && token) {
      const userObj = JSON.parse(user);
      
      if (userObj.userRole === 'Admin') {
        return true;
      }
    }
    
    this.router.navigate(['/dashboard/admin']);
    return false;
  }
}