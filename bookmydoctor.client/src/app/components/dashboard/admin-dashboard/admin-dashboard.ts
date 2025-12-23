import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboardComponent {
  constructor(private router: Router) {}

  viewUsers() {
    console.log('ViewUsers clicked');
    const user = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    console.log('User data:', user);
    console.log('Token exists:', !!token);
    
    if (user) {
      const userData = JSON.parse(user);
      console.log('User role:', userData.userRole);
    }
    
    this.router.navigate(['/admin/patients']);
  }

  viewDoctors() {
    this.router.navigate(['/admin/doctors']);
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/auth/login']);
  }
}