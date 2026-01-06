import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Notification, NotificationType } from '../../core/models/notification.model';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notification-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-model.html',
  styleUrls: ['./notification-model.scss']
})
export class NotificationModalComponent implements OnInit {
  isOpen = false;
  notifications: Notification[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    // Don't load notifications on init
  }

  openModal() {
    this.isOpen = true;
    this.loadNotifications();
  }

  closeModal() {
    this.isOpen = false;
    // Emit event to update header count
    window.dispatchEvent(new CustomEvent('notificationsUpdated'));
  }

  loadNotifications() {
    this.notificationService.getNotifications().subscribe({
      next: (notifications) => {
        this.notifications = notifications;
      },
      error: (error) => {
        console.error('Error loading notifications:', error);
      }
    });
  }

  markAsRead(notificationId: number) {
    this.notificationService.deleteNotification(notificationId).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== notificationId);
        window.dispatchEvent(new CustomEvent('notificationsUpdated'));
      },
      error: (error) => {
        console.error('Error deleting notification:', error);
      }
    });
  }

  markAllAsRead() {
    const deletePromises = this.notifications.map(n => 
      this.notificationService.deleteNotification(n.id).toPromise()
    );
    
    Promise.all(deletePromises).then(() => {
      this.notifications = [];
      window.dispatchEvent(new CustomEvent('notificationsUpdated'));
    }).catch((error) => {
      console.error('Error deleting all notifications:', error);
    });
  }

  getNotificationTypeClass(type: NotificationType): string {
    return type;
  }

  getNotificationIcon(type: NotificationType): string {
    switch (type) {
      case 'appointment':
        return 'M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM7 10h5v5H7z';
      case 'reminder':
        return 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm3.5 6L12 10.5 8.5 8 12 5.5 15.5 8zM12 17.5L8.5 15 12 12.5 15.5 15 12 17.5z';
      default:
        return 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 17h-2v-6h2v6zm0-8h-2V7h2v4z';
    }
  }

  formatTime(date: Date): string {
    if (!date) return 'Unknown';
    
    const now = new Date();
    const notificationDate = new Date(date);
    const diff = now.getTime() - notificationDate.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (minutes < 60) return `${minutes}m ago`;
    if (hours < 24) return `${hours}h ago`;
    return `${days}d ago`;
  }
}