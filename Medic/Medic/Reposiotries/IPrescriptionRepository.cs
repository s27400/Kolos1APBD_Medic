using Medic.Models;

namespace Medic.Reposiotries;

public interface IPrescriptionRepository
{
    public Task<List<Prescription>> GetAllPrescription(string? doctorName);

    public Task<bool> IsPatientExists(int idPatient);

    public Task<bool> IsDoctorExists(int idDoctor);

    public Task<Prescription> AddPrescription(PrescriptionDTO dto);

    public bool DateVeryfivation(DateOnly date, DateOnly dateDue);
}