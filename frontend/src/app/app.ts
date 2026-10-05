import { Component, computed, inject, signal } from '@angular/core';
import { ClinicApi } from './clinic-api';
import { Appointment, Doctor } from './models';

export interface Slot {
  time: string;
  start: string;
  status: 'Free' | 'Booked' | 'Past';
  appointment?: Appointment;
}

function pad(n: number): string {
  return String(n).padStart(2, '0');
}

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private api = inject(ClinicApi);

  doctors = signal<Doctor[]>([]);
  selectedDoctorId = signal<number | null>(null);
  selectedDate = signal<string>('');
  appointments = signal<Appointment[]>([]);

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
}