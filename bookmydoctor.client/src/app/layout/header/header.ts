import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit, OnDestroy {
  isLoggedIn = false;
  userName = '';
  userRole = '';
  showDropdown = false;
  dashboardTitle = 'BookMyDoctor';
  private routerSubscription?: Subscription;

  constructor(
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.checkLoginStatus();
    this.updateDashboardTitle();
    
    // Listen to route changes to update login status and title
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkLoginStatus();
        this.updateDashboardTitle();
      });
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  checkLoginStatus() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (token && user) {
      this.isLoggedIn = true;
      const userData = JSON.parse(user);
      this.userName = userData.userName || 'User';
      this.userRole = userData.userRole || 'Guest';
    } else {
      this.isLoggedIn = false;
      this.userName = '';
      this.userRole = '';
    }
  }

  updateDashboardTitle() {
    const currentUrl = this.router.url;
    if (currentUrl.includes('/dashboard/admin')) {
      this.dashboardTitle = 'Admin Dashboard';
    } else if (currentUrl.includes('/dashboard/doctor')) {
      this.dashboardTitle = 'Doctor Dashboard';
    } else if (currentUrl.includes('/dashboard/patient')) {
      this.dashboardTitle = 'Patient Dashboard';
    } else if (currentUrl.includes('/admin/patients')) {
      this.dashboardTitle = 'Patient Management';
    } else if (currentUrl.includes('/admin/doctors')) {
      this.dashboardTitle = 'Doctor Management';
    } else {
      this.dashboardTitle = 'BookMyDoctor';
    }
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.isLoggedIn = false;
    this.showDropdown = false;
    this.toastr.warning('Logged out successfully!');
    this.router.navigate(['/auth/login']);
  }
}
