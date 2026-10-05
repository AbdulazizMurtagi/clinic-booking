using ClinicBooking.Api.Data;
using ClinicBooking.Api.Models;
using ClinicBooking.Api.Services;

namespace ClinicBooking.Tests;

public class BookingServiceTests
{
    // Pretend "now" for every test: Saturday 3 Oct 2026 at 17:00
    private readonly DateTime _now = new DateTime(2026, 10, 3, 17, 0, 0);

    private readonly DoctorRepository _doctors = new DoctorRepository();
    private readonly PatientRepository _patients = new PatientRepository();
    private readonly AppointmentRepository _appointments = new AppointmentRepository();
    private readonly BookingService _service;

    // Runs before EVERY test, so each test starts with a fresh office
    public BookingServiceTests()
    {
        _service = new BookingService(_doctors, _patients, _appointments);
        _patients.Add(new Patient { FullName = "Sara Ali", PhoneNumber = "55512345", DateOfBirth = new DateOnly(2000, 9, 9) });
        _patients.Add(new Patient { FullName = "Omar Khalid", PhoneNumber = "55598765", DateOfBirth = new DateOnly(1990, 3, 12) });
    }

    // ---------- Time rules ----------

    [Fact]
    public void Accepts_Sunday_at_9_00()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Null(error);
    }

    [Fact]
    public void Rejects_time_not_on_hour_or_half_hour()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 15, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Equal("Appointments must start on the hour or the half hour.", error);
    }

    [Fact]
    public void Rejects_Friday()
    {
        DateTime start = new DateTime(2026, 10, 9, 10, 0, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Equal("The clinic is closed on Fridays and Saturdays.", error);
    }

    [Fact]
    public void Rejects_Saturday()
    {
        DateTime start = new DateTime(2026, 10, 10, 10, 0, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Equal("The clinic is closed on Fridays and Saturdays.", error);
    }

    [Fact]
    public void Rejects_before_opening_at_7_30()
    {
        DateTime start = new DateTime(2026, 10, 4, 7, 30, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Equal("Appointments must be between 8:00 and 16:00.", error);
    }

    [Fact]
    public void Accepts_last_slot_at_15_30()
    {
        DateTime start = new DateTime(2026, 10, 4, 15, 30, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Null(error);
    }

    [Fact]
    public void Rejects_16_00_because_it_ends_after_closing()
    {
        DateTime start = new DateTime(2026, 10, 4, 16, 0, 0);

        string? error = _service.CheckTimeRules(start, _now);

        Assert.Equal("Appointments must be between 8:00 and 16:00.", error);
    }

    [Fact]
    public void Rejects_time_in_the_past()
    {
        DateTime start = new DateTime(2026, 10, 5, 9, 0, 0);
        DateTime laterThatDay = new DateTime(2026, 10, 5, 12, 0, 0);

        string? error = _service.CheckTimeRules(start, laterThatDay);

        Assert.Equal("Appointments can't be booked in the past.", error);
    }

    // ---------- Booking ----------

    [Fact]
    public void Book_succeeds_for_a_free_valid_slot()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);

        BookingResult result = _service.Book(1, 1, start, _now);

        Assert.Equal(BookingStatus.Success, result.Status);
        Assert.NotNull(result.Appointment);
    }

    [Fact]
    public void Book_rejects_same_doctor_at_same_time()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);
        _service.Book(1, 1, start, _now);

        BookingResult second = _service.Book(1, 1, start, _now);

        Assert.Equal(BookingStatus.SlotTaken, second.Status);
    }

    [Fact]
    public void Book_allows_different_doctor_at_same_time()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);
        _service.Book(1, 1, start, _now);

        BookingResult other = _service.Book(2, 2, start, _now);

        Assert.Equal(BookingStatus.Success, other.Status);
    }

    [Fact]
    public void Cancelling_frees_the_slot()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);
        BookingResult first = _service.Book(1, 1, start, _now);
        _appointments.Remove(first.Appointment!);

        BookingResult again = _service.Book(1, 1, start, _now);

        Assert.Equal(BookingStatus.Success, again.Status);
    }

    [Fact]
    public void Book_rejects_unknown_doctor()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);

        BookingResult result = _service.Book(99, 1, start, _now);

        Assert.Equal(BookingStatus.DoctorNotFound, result.Status);
    }

    [Fact]
    public void Book_rejects_unknown_patient()
    {
        DateTime start = new DateTime(2026, 10, 4, 9, 0, 0);

        BookingResult result = _service.Book(1, 500, start, _now);

        Assert.Equal(BookingStatus.PatientNotFound, result.Status);
    }

    // ---------- One upcoming appointment per patient ----------

    [Fact]
    public void Book_rejects_second_upcoming_appointment_for_same_patient()
    {
        _service.Book(1, 1, new DateTime(2026, 10, 4, 9, 0, 0), _now);

        BookingResult second = _service.Book(2, 1, new DateTime(2026, 10, 5, 10, 0, 0), _now);

        Assert.Equal(BookingStatus.PatientAlreadyBooked, second.Status);
    }

    [Fact]
    public void Patient_can_book_again_after_cancelling()
    {
        BookingResult first = _service.Book(1, 1, new DateTime(2026, 10, 4, 9, 0, 0), _now);
        _appointments.Remove(first.Appointment!);

        BookingResult again = _service.Book(2, 1, new DateTime(2026, 10, 5, 10, 0, 0), _now);

        Assert.Equal(BookingStatus.Success, again.Status);
    }

    [Fact]
    public void Past_appointment_does_not_block_a_new_booking()
    {
        _appointments.Add(new Appointment { DoctorId = 1, PatientId = 1, StartTime = new DateTime(2026, 10, 1, 9, 0, 0) });

        BookingResult result = _service.Book(1, 1, new DateTime(2026, 10, 4, 9, 0, 0), _now);

        Assert.Equal(BookingStatus.Success, result.Status);
    }
}