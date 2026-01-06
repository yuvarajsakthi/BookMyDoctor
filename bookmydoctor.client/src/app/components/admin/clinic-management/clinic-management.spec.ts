import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClinicManagement } from './clinic-management';

describe('ClinicManagement', () => {
  let component: ClinicManagement;
  let fixture: ComponentFixture<ClinicManagement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClinicManagement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClinicManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
