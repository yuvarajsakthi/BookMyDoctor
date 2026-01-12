import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-nearby-clinics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './nearby-clinics.html',
  styleUrl: './nearby-clinics.scss'
})
export class NearbyClinicsComponent implements OnInit {
  clinics: any[] = [];
  isLoading = false;
  userLocation: { lat: number, lng: number } | null = null;
  selectedClinic: any = null;
  showModal = false;

  constructor(
    private patientService: PatientService,
    private toastr: ToastrService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadClinics();
  }

  loadClinics() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.patientService.getNearbyClinics().pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (response) => {
        this.toastr.success('Clinics loaded successfully');
        this.clinics = response.data || response || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load clinics');
        this.clinics = [];
      }
    });
  }

  findNearby() {
    if (!navigator.geolocation) {
      this.toastr.error('Geolocation is not supported by this browser');
      return;
    }

    this.isLoading = true;
    this.cdr.detectChanges();
    
    navigator.geolocation.getCurrentPosition(
      (position) => {
        this.userLocation = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        this.calculateDistances();
      },
      () => {
        this.toastr.error('Unable to get your location');
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    );
  }

  private calculateDistances() {
    if (!this.userLocation) return;

    this.clinics = this.clinics.map(clinic => {
      const distance = this.getDistance(
        this.userLocation!.lat,
        this.userLocation!.lng,
        clinic.latitude || 0,
        clinic.longitude || 0
      );
      return { ...clinic, distance: distance.toFixed(1) };
    })
    .filter(clinic => parseFloat(clinic.distance) <= 10) // Only show clinics within 10km
    .sort((a, b) => parseFloat(a.distance) - parseFloat(b.distance));

    this.isLoading = false;
    this.cdr.detectChanges();
    
    if (this.clinics.length === 0) {
      this.toastr.info('No clinics found within 10km of your location');
    } else {
      this.toastr.success(`Found ${this.clinics.length} nearby clinics`);
    }
  }

  private getDistance(lat1: number, lng1: number, lat2: number, lng2: number): number {
    const R = 6371; // Earth's radius in km
    const dLat = this.deg2rad(lat2 - lat1);
    const dLng = this.deg2rad(lng2 - lng1);
    const a = 
      Math.sin(dLat/2) * Math.sin(dLat/2) +
      Math.cos(this.deg2rad(lat1)) * Math.cos(this.deg2rad(lat2)) * 
      Math.sin(dLng/2) * Math.sin(dLng/2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
    return R * c;
  }

  private deg2rad(deg: number): number {
    return deg * (Math.PI/180);
  }

  viewDetails(clinic: any) {
    this.selectedClinic = clinic;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedClinic = null;
  }

  bookAppointment(clinic: any) {
    this.router.navigate(['/patient/book-appointment'], { 
      queryParams: { clinicId: clinic.clinicId || clinic.id } 
    });
  }

  formatWorkingHours(clinic: any): string {
    if (!clinic) return 'Hours not available';
    
    const openTime = this.formatTime(clinic.openTime);
    const closeTime = this.formatTime(clinic.closeTime);
    const workingDays = clinic.workingDays || 'Mon-Fri';
    
    return `${workingDays}: ${openTime} - ${closeTime}`;
  }

  private formatTime(timeString: string): string {
    if (!timeString) return '';
    const [hours, minutes] = timeString.split(':');
    const hour = parseInt(hours);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const displayHour = hour % 12 || 12;
    return `${displayHour}:${minutes} ${ampm}`;
  }
}