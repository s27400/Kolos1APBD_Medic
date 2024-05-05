using Medic.Models;
using Medic.Reposiotries;
using Microsoft.AspNetCore.Mvc;

namespace Medic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionsController(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPrescription(string? doctorName)
    {
        return Ok(await _prescriptionRepository.GetAllPrescription(doctorName));
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDTO newPrescription)
    {
        if (!await _prescriptionRepository.IsPatientExists(newPrescription.IdPatientDto))
        {
            return NotFound($"Pacjent o id : {newPrescription.IdPatientDto} nie istnieje w bazie danych");
        }

        if (!await _prescriptionRepository.IsDoctorExists(newPrescription.IdDoctorDto))
        {
            return NotFound($"Doktor o id : {newPrescription.IdDoctorDto} nie istnieje w bazie danych");
        }

        if (!_prescriptionRepository.DateVeryfivation(DateOnly.FromDateTime(newPrescription.DateDto), DateOnly.FromDateTime(newPrescription.DueDateDto)))
        {
            return NotFound("Data wystawienia jest większa niż data ważności recepty");
        }

        return Ok(await _prescriptionRepository.AddPrescription(newPrescription));
    }
}