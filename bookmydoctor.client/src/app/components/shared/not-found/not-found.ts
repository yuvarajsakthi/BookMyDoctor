import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [],
  template: `
    <div class="not-found-container">
      <div class="not-found-content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you're looking for doesn't exist.</p>
        <button class="btn btn-primary" (click)="goToDashboard()">
          Go to Dashboard
        </button>
      </div>
    </div>
  `,
  styles: [`
    .not-found-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      background: var(--bg-primary);
      padding: var(--space-6);
    }
    
    .not-found-content {
      text-align: center;
      background: var(--card-bg);
      padding: var(--space-12);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      max-width: 500px;
    }
    
    h1 {
      font-size: 6rem;
      font-weight: 700;
      color: var(--primary-600);
      margin: 0;
      line-height: 1;
    }
    
    h2 {
      font-size: var(--font-size-2xl);
      color: var(--text-primary);
      margin: var(--space-4) 0;
    }
    
    p {
      color: var(--text-secondary);
      margin-bottom: var(--space-8);
    }
  `]
})
export class NotFoundComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit() {}

  goToDashboard() {
    const user = sessionStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      const role = userData.userRole?.toLowerCase();
      this.router.navigate([`/dashboard/${role}`]);
    } else {
      this.router.navigate(['/auth/login']);
    }
  }
}