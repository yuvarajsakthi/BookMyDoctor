export interface Notification {
  id: number;
  type: NotificationType;
  title: string;
  message: string;
  createdAt: Date;
  isRead: boolean;
}

export type NotificationType = 'appointment' | 'reminder' | 'system';

export interface NotificationModalConfig {
  isOpen: boolean;
  notifications: Notification[];
}