using System.Data.SqlClient;
using Medic.Models;

namespace Medic.Reposiotries;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly IConfiguration _configuration;

    public PrescriptionRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Prescription>> GetAllPrescription(string? doctorName)
    {

        await using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();

        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        
        List<Prescription> result = new List<Prescription>();

        if (doctorName is null)
        {
            doctorName = "DocLastName";
            cmd.CommandText =
                "Select IdPrescription, [Date], DueDate, Doctor.LastName AS 'DocLastName', Patient.LastName AS 'PatLastName'  FROM Prescription INNER JOIN Doctor On Prescription.IdDoctor = Doctor.IdDoctor INNER JOIN Patient ON Prescription.IdPatient = Patient.IdPatient ORDER BY [Date] DESC";

        }
        else
        {
            cmd.CommandText =
                "Select IdPrescription, [Date], DueDate, Doctor.LastName AS 'DocLastName', Patient.LastName AS 'PatLastName'  FROM Prescription INNER JOIN Doctor On Prescription.IdDoctor = Doctor.IdDoctor INNER JOIN Patient ON Prescription.IdPatient = Patient.IdPatient WHERE Doctor.LastName = @doctorName ORDER BY [Date] DESC";
            cmd.Parameters.AddWithValue("@doctorName", doctorName);
        }

        

        var dr = await cmd.ExecuteReaderAsync();


        while (await dr.ReadAsync())
        {
            result.Add(new Prescription()
            {
                IdPrescription = (int)dr["IdPrescription"],
                Date = DateOnly.FromDateTime((DateTime)dr["Date"]),
                DueDate = DateOnly.FromDateTime((DateTime)dr["DueDate"]),
                DoctorLastName = dr["DocLastName"].ToString(),
                PatientLastName = dr["PatLastName"].ToString(),
            });
        }

        return result;
    }

    public async Task<bool> IsPatientExists(int idPatient)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Patient WHERE IdPatient = @idP";
        cmd.Parameters.AddWithValue("@idP", idPatient);

        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> IsDoctorExists(int idDoctor)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Doctor WHERE IdDoctor = @idD";
        cmd.Parameters.AddWithValue("@idD", idDoctor);

        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return true;
        }

        return false;
    }

    public async Task<Prescription> AddPrescription(PrescriptionDTO dto)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "INSERT INTO Prescription([Date], DueDate, IdPatient, IdDoctor) VALUES (@date, @dueDate, @idP, @idD); SELECT @@IDENTITY AS ID;";

        cmd.Parameters.AddWithValue("@date", dto.DateDto);
        cmd.Parameters.AddWithValue("@dueDate", dto.DueDateDto);
        cmd.Parameters.AddWithValue("@idP", dto.IdPatientDto);
        cmd.Parameters.AddWithValue("@idD", dto.IdDoctorDto);

        var createdId = await cmd.ExecuteScalarAsync();

        if (createdId is null)
        {
            return new Prescription()
            {
                IdPrescription = -999,
                Date = DateOnly.MaxValue,
                DoctorLastName = "aaa",
                DueDate = DateOnly.MaxValue,
                PatientLastName = "bbb",
            };
        }

        decimal resultId = (decimal)createdId;
        cmd.Parameters.Clear();
        cmd.CommandText = "SELECT LastName FROM Doctor WHERE IdDoctor = @docId";
        cmd.Parameters.AddWithValue("@docId", dto.IdDoctorDto);
        var doctorName = await cmd.ExecuteScalarAsync();
        string resultDoctorName = doctorName.ToString();
        
        cmd.Parameters.Clear();
        cmd.CommandText = "SELECT LastName From Patient WHERE IdPatient = @patId";
        cmd.Parameters.AddWithValue("@patId", dto.IdPatientDto);
        var patientName = await cmd.ExecuteScalarAsync();
        string resultPatientName = patientName.ToString();

        return new Prescription()
        {
            Date = DateOnly.FromDateTime(dto.DateDto),
            DoctorLastName = resultDoctorName,
            DueDate = DateOnly.FromDateTime(dto.DueDateDto),
            IdPrescription = resultId,
            PatientLastName = resultPatientName,
        };
    }

    public bool DateVeryfivation(DateOnly date, DateOnly dateDue)
    {
        if (dateDue > date)
        {
            return true;
        }
        return false;
    }
}