import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorNotifications } from './doctor-notifications';

describe('DoctorNotifications', () => {
  let component: DoctorNotifications;
  let fixture: ComponentFixture<DoctorNotifications>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorNotifications]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorNotifications);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
