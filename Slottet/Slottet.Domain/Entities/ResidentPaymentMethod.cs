namespace Slottet.Domain.Entities
{
    public class ResidentPaymentMethod
    {
        public Resident ResidentID { get; set; }
        public PaymentMethod PaymentMethodID { get; set; }
    }
}
