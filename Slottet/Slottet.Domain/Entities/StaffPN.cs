namespace Slottet.Domain.Entities
{
    public class StaffPN
    {
        public Guid StaffID { get; set; }
        public Staff Staff { get; set; }

        public Guid PNID { get; set; }
        public PN PN { get; set; }


    }
}
