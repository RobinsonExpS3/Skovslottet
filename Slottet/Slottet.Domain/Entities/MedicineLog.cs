namespace Slottet.Domain.Entities
{
    /// <summary>
    /// Registrerer om en given medicin er udleveret på en bestemt dato.
    /// En post oprettes per dag per medicinindgang når medicinen gives.
    /// </summary>
    public class MedicineLog
    {
        public Guid MedicineLogID { get; set; }

        /// <summary>Den dato denne log-post gælder for.</summary>
        public DateOnly Date { get; set; }

        /// <summary>Tidspunkt medicinen blev givet. Null = ikke givet endnu.</summary>
        public DateTime? GivenTime { get; set; }

        /// <summary>Tidspunkt registreringen fandt sted.</summary>
        public DateTime? RegisteredTime { get; set; }

        public Guid MedicineID { get; set; }
        public Medicine Medicine { get; set; }
    }
}
