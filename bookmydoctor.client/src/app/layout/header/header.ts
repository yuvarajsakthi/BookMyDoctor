import { Component, OnInit, OnDestroy, HostListener, Output, EventEmitter } from '@angular/core';

import { Router, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit, OnDestroy {
  @Output() mobileMenuToggle = new EventEmitter<void>();
  
  isLoggedIn = false;
  userName = '';
  userRole = '';
  showDropdown = false;
  dashboardTitle = 'BookMyDoctor';
  isMobile = false;
  notificationCount = 0;
  private routerSubscription?: Subscription;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private notificationService: NotificationService
  ) {}

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.checkScreenSize();
  }

  ngOnInit() {
    this.checkLoginStatus();
    this.updateDashboardTitle();
    this.checkScreenSize();
    
    // Listen to route changes to update login status and title
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkLoginStatus();
        this.updateDashboardTitle();
      });

    // Listen for notification updates
    window.addEventListener('notificationsUpdated', () => {
      // Just reset count when notifications are updated
      this.notificationCount = 0;
    });
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  checkScreenSize() {
    this.isMobile = window.innerWidth <= 768;
  }

  checkLoginStatus() {
    const token = sessionStorage.getItem('token');
    const user = sessionStorage.getItem('user');
    
    if (token && user) {
      this.isLoggedIn = true;
      const userData = JSON.parse(user);
      this.userName = userData.userName || 'User';
      this.userRole = userData.userRole || 'Guest';
    } else {
      this.isLoggedIn = false;
      this.userName = '';
      this.userRole = '';
      this.notificationCount = 0;
    }
  }

  getUserInitials(): string {
    if (!this.userName) return 'U';
    const names = this.userName.split(' ');
    if (names.length >= 2) {
      return (names[0][0] + names[1][0]).toUpperCase();
    }
    return this.userName[0].toUpperCase();
  }

  toggleMobileMenu() {
    this.mobileMenuToggle.emit();
  }

  updateDashboardTitle() {
    this.dashboardTitle = 'BookMyDoctor';
  }

  logout() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');
    this.isLoggedIn = false;
    this.showDropdown = false;
    this.toastr.warning('Logged out successfully!');
    this.router.navigate(['/auth/login']);
  }

  navigateToLogin() {
    this.router.navigate(['/auth/login']);
  }

  navigateToRegister() {
    this.router.navigate(['/auth/register']);
  }

  navigateToHome() {
    this.router.navigate(['/']);
  }

  openNotifications() {
    // Navigate to notifications page instead of opening modal
    if (this.userRole === 'Patient') {
      this.router.navigate(['/patient/notifications']);
    } else if (this.userRole === 'Doctor') {
      this.router.navigate(['/doctor/notifications']);
    }
    this.notificationCount = 0;
  }

  loadNotificationCount() {
    if (this.userRole !== 'Admin') {
      this.notificationService.getNotifications().subscribe({
        next: (notifications) => {
          this.notificationCount = notifications.length;
        },
        error: () => {
          this.notificationCount = 0;
        }
      });
    }
  }
}
