using ClinicBooking.Api.Data;
using ClinicBooking.Api.Models;
using ClinicBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly BookingService _bookingService;
    private readonly AppointmentRepository _appointments;

    public AppointmentsController(BookingService bookingService, AppointmentRepository appointments)
    {
        _bookingService = bookingService;
        _appointments = appointments;
    }

    [HttpPost]
    public ActionResult<Appointment> Book(CreateAppointmentRequest request)
    {
        BookingResult result = _bookingService.Book(
            request.DoctorId!.Value,
            request.PatientId!.Value,
            request.StartTime!.Value,
            DateTime.Now);

        if (result.Status == BookingStatus.Success)
        {
            return StatusCode(201, result.Appointment);
        }

        if (result.Status == BookingStatus.DoctorNotFound || result.Status == BookingStatus.PatientNotFound)
        {
            return Problem(detail: result.ErrorMessage, statusCode: 404);
        }

        if (result.Status == BookingStatus.SlotTaken)
        {
            return Problem(detail: result.ErrorMessage, statusCode: 409);
        }

        return Problem(detail: result.ErrorMessage, statusCode: 400);
    }

    [HttpGet]
    public List<Appointment> GetByDoctorAndDate([FromQuery] int doctorId, [FromQuery] DateOnly date)
    {
        return _appointments.GetByDoctorAndDate(doctorId, date);
    }

    [HttpDelete("{id}")]
    public IActionResult Cancel(int id)
    {
        Appointment? appointment = _appointments.GetById(id);

        if (appointment == null)
        {
            return Problem(detail: "Appointment not found.", statusCode: 404);
        }

        _appointments.Remove(appointment);
        return NoContent();
    }
}