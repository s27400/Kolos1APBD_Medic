using System.Runtime.InteropServices.JavaScript;

namespace Medic.Models;

public class Prescription
{
    public decimal IdPrescription { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly DueDate { get; set; }
    public string DoctorLastName { get; set; }
    public string PatientLastName { get; set; }
}