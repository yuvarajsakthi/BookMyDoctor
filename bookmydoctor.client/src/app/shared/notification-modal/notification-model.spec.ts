import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotificationModalComponent } from './notification-modal.component';

describe('NotificationModalComponent', () => {
  let component: NotificationModalComponent;
  let fixture: ComponentFixture<NotificationModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NotificationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should open modal', () => {
    component.openModal();
    expect(component.isOpen).toBe(true);
  });

  it('should close modal', () => {
    component.closeModal();
    expect(component.isOpen).toBe(false);
  });

  it('should mark notification as read', () => {
    component.loadNotifications();
    const unreadNotification = component.notifications.find(n => !n.isRead);
    if (unreadNotification) {
      component.markAsRead(unreadNotification.id);
      expect(unreadNotification.isRead).toBe(true);
    }
  });

  it('should mark all notifications as read', () => {
    component.loadNotifications();
    component.markAllAsRead();
    expect(component.notifications.every(n => n.isRead)).toBe(true);
  });
});