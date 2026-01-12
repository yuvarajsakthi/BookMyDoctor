import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { NotificationService } from '../../../core/services/notification.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-doctor-notifications',
  standalone: true,
  imports: [
    CommonModule,
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
  isLoading = false;

  constructor(
    private notificationService: NotificationService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.notificationService.getDoctorNotifications()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response) => {
          this.notifications = response || [];
        },
        error: () => {
          this.notifications = [];
          this.toastr.error('Failed to load notifications');
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
        this.toastr.success('Notification marked as read');
      },
      error: () => {
        this.toastr.error('Failed to mark notification as read');
      }
    });
  }

  markAllAsRead() {
    const unreadNotifications = this.notifications.filter(n => !n.isRead);
    if (unreadNotifications.length === 0) return;

    unreadNotifications.forEach(notification => {
      this.notificationService.markAsRead(notification.id).subscribe({
        next: () => {
          notification.isRead = true;
        },
        error: () => {
          this.toastr.error('Failed to mark notification as read');
        }
      });
    });
    
    this.toastr.success('All notifications marked as read');
  }

  deleteNotification(notificationId: number) {
    this.notificationService.deleteNotification(notificationId).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== notificationId);
        this.toastr.success('Notification deleted');
      },
      error: () => {
        this.toastr.error('Failed to delete notification');
      }
    });
  }

  getUnreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  getNotificationIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'new_appointment': return 'event';
      case 'appointment_cancelled': return 'event_busy';
      case 'appointment_rescheduled': return 'update';
      case 'payment_received': return 'payment';
      case 'reminder': return 'alarm';
      default: return 'notifications';
    }
  }

  getNotificationType(type: string): string {
    switch (type?.toLowerCase()) {
      case 'new_appointment': return 'New Appointment';
      case 'appointment_cancelled': return 'Cancelled';
      case 'appointment_rescheduled': return 'Rescheduled';
      case 'payment_received': return 'Payment';
      case 'reminder': return 'Reminder';
      default: return 'General';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffInHours = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

    if (diffInHours < 1) {
      const minutes = Math.floor(diffInHours * 60);
      return minutes <= 0 ? 'Just now' : `${minutes}m ago`;
    } else if (diffInHours < 24) {
      return `${Math.floor(diffInHours)}h ago`;
    } else {
      return date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    }
  }
}