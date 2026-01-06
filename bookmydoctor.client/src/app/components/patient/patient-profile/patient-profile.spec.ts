import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientProfileComponent } from './patient-profile';

describe('PatientProfileComponent', () => {
  let component: PatientProfileComponent;
  let fixture: ComponentFixture<PatientProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientProfileComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PatientProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form as disabled', () => {
    expect(component.profileForm.disabled).toBe(true);
  });

  it('should enable form when editing', () => {
    component.toggleEdit();
    expect(component.isEditing).toBe(true);
    expect(component.profileForm.enabled).toBe(true);
  });

  it('should disable form when canceling edit', () => {
    component.toggleEdit();
    component.cancelEdit();
    expect(component.isEditing).toBe(false);
    expect(component.profileForm.disabled).toBe(true);
  });
});