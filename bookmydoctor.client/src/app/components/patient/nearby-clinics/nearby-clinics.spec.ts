import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NearbyClinicsComponent } from './nearby-clinics';

describe('NearbyClinicsComponent', () => {
  let component: NearbyClinicsComponent;
  let fixture: ComponentFixture<NearbyClinicsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NearbyClinicsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NearbyClinicsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});