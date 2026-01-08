import { Component, OnInit } from '@angular/core';

import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { PaymentService } from '../../../core/services/payment.service';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule
],
  templateUrl: './book-appointment.html',
  styleUrl: './book-appointment.scss'
})
export class BookAppointmentComponent implements OnInit {
  appointmentForm!: FormGroup;
  clinics: any[] = [];
  doctors: any[] = [];
  availableTimeSlots: any[] = [];
  isLoading = false;
  isSubmitting = false;
  isLoadingDoctors = false;
  isLoadingSlots = false;
  selectedClinic: any = null;
  selectedDoctor: any = null;
  minDate = new Date();

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService,
    private paymentService: PaymentService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadData();
    this.setupFormSubscriptions();
  }

  initForm() {
    this.appointmentForm = this.fb.group({
      clinicId: ['', Validators.required],
      doctorId: ['', Validators.required],
      date: ['', Validators.required],
      startTime: ['', Validators.required],
      reason: ['']
    });
  }

  setupFormSubscriptions() {
    // Load doctors when clinic changes
    this.appointmentForm.get('clinicId')?.valueChanges.subscribe((clinicId) => {
      if (clinicId) {
        this.selectedClinic = this.clinics.find(c => c.clinicId === clinicId);
        this.loadDoctorsByClinic(clinicId);
      } else {
        this.doctors = [];
        this.selectedClinic = null;
      }
      this.appointmentForm.patchValue({ doctorId: '', startTime: '' });
      this.availableTimeSlots = [];
      this.selectedDoctor = null;
    });
    
    // Update selected doctor and available times when doctor changes
    this.appointmentForm.get('doctorId')?.valueChanges.subscribe((doctorId) => {
      if (doctorId) {
        this.selectedDoctor = this.doctors.find(d => d.userId === doctorId);
      } else {
        this.selectedDoctor = null;
      }
      this.updateAvailableTimeSlots();
    });
    
    this.appointmentForm.get('date')?.valueChanges.subscribe(() => {
      this.updateAvailableTimeSlots();
    });
  }

  loadData() {
    this.isLoading = true;
    this.patientService.getNearbyClinics().subscribe({
      next: (response) => {
        this.clinics = response?.data || [];
        
        // Pre-select clinic if passed via query params
        const clinicId = this.route.snapshot.queryParams['clinicId'];
        if (clinicId) {
          this.appointmentForm.patchValue({ clinicId: parseInt(clinicId) });
        }
        
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load clinics');
        this.isLoading = false;
      }
    });
  }

  loadDoctorsByClinic(clinicId: number) {
    this.isLoadingDoctors = true;
    this.patientService.getDoctorsByClinic(clinicId).subscribe({
      next: (response) => {
        this.doctors = response?.data || [];
        this.isLoadingDoctors = false;
        
        if (this.doctors.length === 0) {
          this.toastr.info('No doctors available at this clinic');
        } else {
          this.doctors.forEach(doctor => {
            this.loadDoctorAvailability(doctor.userId);
          });
        }
      },
      error: () => {
        this.toastr.error('Failed to load doctors');
        this.doctors = [];
        this.isLoadingDoctors = false;
      }
    });
  }

  loadDoctorAvailability(doctorId: number) {
    this.patientService.getDoctorAvailability(doctorId).subscribe({
      next: (response) => {
        const doctor = this.doctors.find(d => d.userId === doctorId);
        if (doctor && response?.data) {
          const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
          const availableDays = response.data
            .filter((a: any) => a.isActive)
            .map((a: any) => dayNames[a.dayOfWeek])
            .join(', ');
          doctor.availableDays = availableDays || 'No schedule set';
        }
      },
      error: () => {}
    });
  }

  updateAvailableTimeSlots() {
    const doctorId = this.appointmentForm.get('doctorId')?.value;
    const selectedDate = this.appointmentForm.get('date')?.value;
    
    if (!doctorId || !selectedDate) {
      this.availableTimeSlots = [];
      return;
    }
    
    // Check if selected date is in the past
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (selectedDate < today) {
      this.toastr.warning('Please select a future date');
      this.availableTimeSlots = [];
      return;
    }
    
    this.isLoadingSlots = true;
    const dateString = selectedDate.toISOString().split('T')[0];
    this.patientService.getAvailableSlots(doctorId, dateString).subscribe({
      next: (response) => {
        this.availableTimeSlots = response?.data || [];
        this.isLoadingSlots = false;
        
        // Reset time selection when slots change
        this.appointmentForm.patchValue({ startTime: '' });
        
        if (this.availableTimeSlots.length === 0) {
          this.toastr.info('No available slots for the selected date. Try another date.');
        }
      },
      error: () => {
        this.toastr.error('Failed to load available slots');
        this.availableTimeSlots = [];
        this.isLoadingSlots = false;
      }
    });
  }

  onSubmit() {
    if (this.appointmentForm.invalid) {
      this.toastr.warning('Please fill in all required fields');
      return;
    }

    this.isSubmitting = true;
    const formValue = this.appointmentForm.value;
    const appointmentData = {
      clinicId: formValue.clinicId,
      doctorId: formValue.doctorId,
      appointmentDate: formValue.date.toISOString().split('T')[0],
      startTime: formValue.startTime,
      reason: formValue.reason || ''
    };

    this.patientService.bookAppointment(appointmentData).subscribe({
      next: (response) => {
        this.toastr.success('Appointment booked successfully!');
        
        // Ask if user wants to pay advance
        if (confirm('Would you like to pay the consultation fee now?')) {
          this.processAdvancePayment(response.data.appointmentId);
        } else {
          this.router.navigate(['/patient/appointments']);
        }
      },
      error: (error) => {
        const errorMessage = error?.error?.message || 'Failed to book appointment';
        this.toastr.error(errorMessage);
        this.isSubmitting = false;
      }
    });
  }

  processAdvancePayment(appointmentId: number) {
    const doctor = this.selectedDoctor;
    if (!doctor?.consultationFee) {
      this.router.navigate(['/patient/appointments']);
      return;
    }

    this.paymentService.createPayment({
      appointmentId: appointmentId,
      amount: doctor.consultationFee
    }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.openRazorpayCheckout(response.data);
        }
      },
      error: () => {
        this.toastr.error('Failed to create payment order');
        this.router.navigate(['/patient/appointments']);
      }
    });
  }

  private openRazorpayCheckout(paymentData: any) {
    const options = {
      key: 'rzp_test_key', // Replace with actual key
      amount: paymentData.amount * 100,
      currency: paymentData.currency,
      name: 'BookMyDoctor',
      description: paymentData.description,
      order_id: paymentData.razorpayOrderId,
      prefill: {
        name: 'Patient Name',
        email: 'patient@example.com'
      },
      theme: {
        color: '#3498db'
      }
    };

    this.paymentService.openRazorpay(options)
      .then((response: any) => {
        this.verifyPayment(response);
      })
      .catch(() => {
        this.toastr.info('Payment cancelled');
        this.router.navigate(['/patient/appointments']);
      });
  }

  private verifyPayment(razorpayResponse: any) {
    const verifyRequest = {
      razorpayOrderId: razorpayResponse.razorpay_order_id,
      razorpayPaymentId: razorpayResponse.razorpay_payment_id,
      razorpaySignature: razorpayResponse.razorpay_signature
    };

    this.paymentService.verifyPayment(verifyRequest).subscribe({
      next: () => {
        this.toastr.success('Payment completed successfully!');
        this.router.navigate(['/patient/appointments']);
      },
      error: () => {
        this.toastr.error('Payment verification failed');
        this.router.navigate(['/patient/appointments']);
      }
    });
  }

  goBack() {
    this.router.navigate(['/patient/appointments']);
  }

  isWeekend(date: Date): boolean {
    const day = date.getDay();
    return day === 0 || day === 6; // Sunday = 0, Saturday = 6
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date >= today;
  }
}