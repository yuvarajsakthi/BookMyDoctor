import { Component, OnInit, OnDestroy, HostListener, Output, EventEmitter, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { NotificationModalComponent } from '../../shared/notification-modal/notification-model';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, NotificationModalComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit, OnDestroy {
  @Output() mobileMenuToggle = new EventEmitter<void>();
  @ViewChild(NotificationModalComponent) notificationModal!: NotificationModalComponent;
  
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
      if (this.isLoggedIn) {
        this.loadNotificationCount();
      }
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
    
    const wasLoggedIn = this.isLoggedIn;
    
    if (token && user) {
      this.isLoggedIn = true;
      const userData = JSON.parse(user);
      this.userName = userData.userName || 'User';
      this.userRole = userData.userRole || 'Guest';
      
      // Only load notifications if just logged in or role changed
      if (!wasLoggedIn || this.userRole !== userData.userRole) {
        this.loadNotificationCount();
      }
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
    this.notificationModal.openModal();
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
