export interface Notification {
  notificationId: number;
  userId: number;
  message: string;
  sentAt: string;
}

export type NotificationType = 'appointment' | 'reminder' | 'system';

export interface NotificationModalConfig {
  isOpen: boolean;
  notifications: Notification[];
}