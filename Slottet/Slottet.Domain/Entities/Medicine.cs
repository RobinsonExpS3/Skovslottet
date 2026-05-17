namespace Slottet.Domain.Entities
{
    public class Medicine
    {
        public Guid MedicineID { get; set; }

        /// <summary>Gentagende daglig medicintid (kun klokkeslæt, ingen dato).</summary>
        public TimeOnly ScheduledTime { get; set; }

        public Guid ResidentID { get; set; }
        public Resident Resident { get; set; }

        /// <summary>Soft delete flag — inaktive medicin-rækker skjules som standard via query filter.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Daglige log-poster — én pr. dato der registrerer om medicinen er givet.</summary>
        public ICollection<MedicineLog> MedicineLogs { get; set; } = new List<MedicineLog>();
    }
}
