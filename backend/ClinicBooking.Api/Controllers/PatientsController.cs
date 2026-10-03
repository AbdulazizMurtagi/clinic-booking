using ClinicBooking.Api.Data;
using ClinicBooking.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientRepository _repository;

    public PatientsController(PatientRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public List<Patient> GetAll()
    {
        return _repository.GetAll();
    }

        [HttpPost]
    public ActionResult<Patient> Create(CreatePatientRequest request)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        if (request.DateOfBirth > today)
        {
            ModelState.AddModelError("DateOfBirth", "Date of birth can't be in the future.");
            return ValidationProblem(ModelState);
        }

        Patient patient = new Patient
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth!.Value
        };

        Patient created = _repository.Add(patient);
        return StatusCode(201, created);
    }
}