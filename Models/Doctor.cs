namespace HospitalAppointment.Models
{
    // This class stores information about a doctor.
    public class Doctor
    {
        // Stores the unique ID of the doctor.
        public int DoctorId { get; set; }

        // Stores the doctor's name.
        public string DoctorName { get; set; } = string.Empty;

        // Stores the doctor's specialization.
        public string Specialization { get; set; } = string.Empty;

        // Stores the doctor's years of experience.
        public int Experience { get; set; }

        // Stores the doctor's consultation fee.
        public decimal ConsultationFee { get; set; }
    }
}