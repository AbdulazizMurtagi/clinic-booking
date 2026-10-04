import { Component, inject, signal } from '@angular/core';
import { ClinicApi } from './clinic-api';
import { Doctor } from './models';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private api = inject(ClinicApi);

  doctors = signal<Doctor[]>([]);

  constructor() {
    this.api.getDoctors().subscribe(list => this.doctors.set(list));
  }
}