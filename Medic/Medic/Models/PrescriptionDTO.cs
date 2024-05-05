using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace Medic.Models;

public class PrescriptionDTO
{
    [Required]
    public DateTime DateDto { get; set; }
    [Required]
    public DateTime DueDateDto { get; set; }
    [Required]
    [Range(1, Int32.MaxValue)]
    public int IdPatientDto { get; set; }
    [Required]
    [Range(1, Int32.MaxValue)]
    public int IdDoctorDto { get; set; }
    
}