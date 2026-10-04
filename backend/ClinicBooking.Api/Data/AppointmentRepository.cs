using ClinicBooking.Api.Models;

namespace ClinicBooking.Api.Data;

public class AppointmentRepository
{
    private readonly List<Appointment> _appointments = new List<Appointment>();
    private int _nextId = 1;

    public Appointment Add(Appointment appointment)
    {
        appointment.Id = _nextId;
        _nextId++;
        _appointments.Add(appointment);
        return appointment;
    }

    public void Remove(Appointment appointment)
    {
        _appointments.Remove(appointment);
    }

    public Appointment? GetById(int id)
    {
        foreach (Appointment appointment in _appointments)
        {
            if (appointment.Id == id)
            {
                return appointment;
            }
        }

        return null;
    }

    public bool IsSlotTaken(int doctorId, DateTime startTime)
    {
        foreach (Appointment appointment in _appointments)
        {
            if (appointment.DoctorId == doctorId && appointment.StartTime == startTime)
            {
                return true;
            }
        }

        return false;
    }

    public List<Appointment> GetByDoctorAndDate(int doctorId, DateOnly date)
    {
        List<Appointment> result = new List<Appointment>();

        foreach (Appointment appointment in _appointments)
        {
            if (appointment.DoctorId == doctorId && DateOnly.FromDateTime(appointment.StartTime) == date)
            {
                result.Add(appointment);
            }
        }
        result.Sort((first, second) => first.StartTime.CompareTo(second.StartTime));

        return result;
    }
}