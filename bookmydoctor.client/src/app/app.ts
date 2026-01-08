import { Component, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { Header } from './layout/header/header';
import { Sidebar } from './layout/sidebar/sidebar';

import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Sidebar],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App implements OnInit, OnDestroy {
  sidebarOpen = false;
  showSidebar = false;
  private routerSubscription?: Subscription;

  constructor(private router: Router) {}

  ngOnInit() {
    this.checkSidebarVisibility();
    
    // Listen to route changes to update sidebar visibility
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkSidebarVisibility();
        this.sidebarOpen = false; // Close mobile sidebar on route change
      });
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  checkSidebarVisibility() {
    const currentUrl = this.router.url;
    const isAuthPage = currentUrl.includes('/auth/');
    const isLoggedIn = !!sessionStorage.getItem('token');
    
    this.showSidebar = !isAuthPage && isLoggedIn;
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  onSidebarOpenChange(isOpen: boolean) {
    this.sidebarOpen = isOpen;
  }
}