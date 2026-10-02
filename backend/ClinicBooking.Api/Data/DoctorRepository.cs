using ClinicBooking.Api.Models;

namespace ClinicBooking.Api.Data;

public class DoctorRepository
{
    private readonly List<Doctor> _doctors = new List<Doctor>
    {
        new Doctor { Id = 1, Name = "Dr. Ahmad Al-Sabah", Specialty = "Cardiology" },
        new Doctor { Id = 2, Name = "Dr. Fatima Al-Ali", Specialty = "Pediatrics" },
        new Doctor { Id = 3, Name = "Dr. Omar Hassan", Specialty = "Dermatology" }
    };

    public List<Doctor> GetAll()
    {
        return _doctors;
    }
}