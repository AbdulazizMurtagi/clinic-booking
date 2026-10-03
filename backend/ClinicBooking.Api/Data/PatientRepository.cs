using ClinicBooking.Api.Models;

namespace ClinicBooking.Api.Data;

public class PatientRepository
{
    private readonly List<Patient> _patients = new List<Patient>();
    private int _nextId = 1;

    public List<Patient> GetAll()
    {
        return _patients;
    }

        public Patient? GetById(int id)
    {
        foreach (Patient patient in _patients)
        {
            if (patient.Id == id)
            {
                return patient;
            }
        }

        return null;
    }

    public Patient Add(Patient patient)
    {
        patient.Id = _nextId;
        _nextId++;
        _patients.Add(patient);
        return patient;
    }
}