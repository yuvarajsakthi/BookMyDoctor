import { Component, OnInit, OnDestroy, HostListener, Input, Output, EventEmitter } from '@angular/core';

import { RouterModule, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  imports: [RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar implements OnInit, OnDestroy {
  @Input() isOpen = false;
  @Output() openChange = new EventEmitter<boolean>();

  sidebarTitle = '';
  isCollapsed = false;
  isMobile = false;
  userRole = '';
  private subscriptions: Subscription[] = [];

  constructor(private router: Router, private toastr: ToastrService) {}

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.checkScreenSize();
  }

  ngOnInit() {
    this.checkScreenSize();
    this.loadUserRole();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  checkScreenSize() {
    this.isMobile = window.innerWidth <= 768;
    if (!this.isMobile && this.isOpen) {
      this.isOpen = false;
      this.openChange.emit(false);
    }
  }

  loadUserRole() {
    const user = sessionStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      this.userRole = userData.userRole || '';
      this.setSidebarTitle();
    }
  }

  isRouteActive(route: string): boolean {
    const currentUrl = this.router.url;
    if (currentUrl.includes('/admin/user-form')) {
      // Check query parameters to determine context
      const urlParams = new URLSearchParams(window.location.search);
      const userType = urlParams.get('type');
      const returnUrl = urlParams.get('returnUrl');
      
      if (route === '/admin/doctors' && (userType === 'doctor' || returnUrl?.includes('doctors'))) {
        return true;
      }
      if (route === '/admin/patients' && (userType === 'patient' || returnUrl?.includes('patients'))) {
        return true;
      }
    }
    return currentUrl.includes(route);
  }

  setSidebarTitle() {
    switch(this.userRole) {
      case 'Admin':
        this.sidebarTitle = 'Admin Dashboard';
        break;
      case 'Doctor':
        this.sidebarTitle = 'Doctor Dashboard';
        break;
      case 'Patient':
        this.sidebarTitle = 'Patient Dashboard';
        break;
      default:
        this.sidebarTitle = 'BookMyDoctor';
    }
  }

  toggleCollapse() {
    if (!this.isMobile) {
      this.isCollapsed = !this.isCollapsed;
    } else {
      this.isOpen = !this.isOpen;
      this.openChange.emit(this.isOpen);
    }
  }

  close() {
    this.isOpen = false;
    this.openChange.emit(false);
  }

  open() {
    this.isOpen = true;
    this.openChange.emit(true);
  }

  logout() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');
    this.toastr.success('Logged out successfully!');
    this.router.navigate(['/auth/login']);
  }
}