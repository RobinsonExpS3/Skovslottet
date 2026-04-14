namespace Slottet.Domain.Entities
{
    public class ResidentPaymentMethod
    {
        public Guid ResidentID { get; set; }
        public Guid PaymentMethodID { get; set; }

        public Resident Resident { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<Resident> Residents { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}
