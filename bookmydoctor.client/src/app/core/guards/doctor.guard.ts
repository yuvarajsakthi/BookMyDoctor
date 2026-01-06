import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class DoctorGuard implements CanActivate {
  constructor(
    private router: Router,
    private jwtHelper: JwtHelperService
  ) {}

  canActivate(): boolean {
    const token = sessionStorage.getItem('token');
    const user = sessionStorage.getItem('user');
    
    if (user && token) {
      const userObj = JSON.parse(user);
      
      if (userObj.userRole === 'Doctor') {
        return true;
      }
    }
    
    this.router.navigate(['/auth/login']);
    return false;
  }
}