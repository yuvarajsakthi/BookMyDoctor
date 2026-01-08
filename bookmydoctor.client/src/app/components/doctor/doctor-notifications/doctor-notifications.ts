import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-doctor-notifications',
  imports: [
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatChipsModule
],
  templateUrl: './doctor-notifications.html',
  styleUrl: './doctor-notifications.scss',
})
export class DoctorNotifications implements OnInit {
  notifications: any[] = [];
  isLoading = true;

  constructor(
    private notificationService: NotificationService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications() {
    this.notificationService.getDoctorNotifications().subscribe({
      next: (response) => {
        this.notifications = response || [];
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load notifications');
        this.isLoading = false;
      }
    });
  }

  markAsRead(notificationId: number) {
    this.notificationService.markAsRead(notificationId).subscribe({
      next: () => {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification) {
          notification.isRead = true;
        }
      },
      error: () => {
        this.toastr.error('Failed to mark notification as read');
      }
    });
  }

  getNotificationIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'new_appointment': return 'event';
      case 'appointment_cancelled': return 'event_busy';
      case 'appointment_rescheduled': return 'update';
      case 'payment_received': return 'payment';
      default: return 'notifications';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
