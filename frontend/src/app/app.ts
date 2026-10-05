import { Component, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClinicApi } from './clinic-api';
import { Appointment, Doctor, Patient } from './models';

export interface Slot {
  time: string;
  start: string;
  status: 'Free' | 'Booked' | 'Past';
  appointment?: Appointment;
}

function pad(n: number): string {
  return String(n).padStart(2, '0');
}

function todayString(): string {
  const d = new Date();
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

// NEW: turns any backend error reply into one readable sentence
function errorMessage(err: HttpErrorResponse): string {
  const body = err.error;
  if (body?.errors) return Object.values(body.errors).flat().join(' ');
  if (body?.detail) return body.detail;
  return 'Something went wrong. Please check that the server is running.';
}

@Component({
  selector: 'app-root',
  imports: [ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private api = inject(ClinicApi);

  today = todayString();

  doctors = signal<Doctor[]>([]);
  selectedDoctorId = signal<number | null>(null);
  selectedDate = signal<string>('');
  appointments = signal<Appointment[]>([]);

  // NEW: patients
  patients = signal<Patient[]>([]);
  selectedPatientId = signal<number | null>(null);
  patientMessage = signal('');
  patientError = signal('');

  // NEW: the patient form and its rules (same rules as CreatePatientRequest)
  patientForm = new FormGroup({
    fullName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2), Validators.maxLength(100)]
    }),
    phoneNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^\+?[0-9]{8,15}$/)]
    }),
    dateOfBirth: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    })
  });

  isClosedDay = computed(() => {
    if (this.selectedDate() === '') return false;
    const day = new Date(this.selectedDate() + 'T00:00:00').getDay();
    return day === 5 || day === 6;
  });

  slots = computed(() => {
    const list: Slot[] = [];
    const now = new Date();

    for (let hour = 8; hour < 16; hour++) {
      for (const minute of [0, 30]) {
        const time = `${pad(hour)}:${pad(minute)}`;
        const start = `${this.selectedDate()}T${time}:00`;
        const appointment = this.appointments().find(a => a.startTime === start);

        let status: Slot['status'] = 'Free';
        if (appointment) status = 'Booked';
        else if (new Date(start) < now) status = 'Past';

        list.push({ time, start, status, appointment });
      }
    }
    return list;
  });

  constructor() {
    this.api.getDoctors().subscribe(list => this.doctors.set(list));
    this.loadPatients();
  }

  selectDoctor(id: string) {
    this.selectedDoctorId.set(Number(id));
    this.loadAppointments();
  }

  selectDate(date: string) {
    this.selectedDate.set(date);
    this.loadAppointments();
  }

  loadAppointments() {
    const doctorId = this.selectedDoctorId();
    const date = this.selectedDate();
    if (doctorId === null || date === '') {
      this.appointments.set([]);
      return;
    }
    this.api.getAppointments(doctorId, date).subscribe(list => this.appointments.set(list));
  }

  // NEW
  loadPatients() {
    this.api.getPatients().subscribe(list => this.patients.set(list));
  }

  // NEW
  selectPatient(id: string) {
    this.selectedPatientId.set(Number(id));
  }

  // NEW
  createPatient() {
    this.patientMessage.set('');
    this.patientError.set('');

    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }

    const { fullName, phoneNumber, dateOfBirth } = this.patientForm.getRawValue();

    if (dateOfBirth > this.today) {
      this.patientError.set("Date of birth can't be in the future.");
      return;
    }

    this.api.createPatient(fullName.trim(), phoneNumber, dateOfBirth).subscribe({
      next: patient => {
        this.patientMessage.set(`${patient.fullName} was added.`);
        this.patientForm.reset();
        this.loadPatients();
        this.selectedPatientId.set(patient.id);
      },
      error: err => this.patientError.set(errorMessage(err))
    });
  }
}