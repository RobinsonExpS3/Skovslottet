using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class ResidentPaymentMethod {
        public Resident ResidentCardID { get; set; }
        public PaymentMethod PaymentMethodID { get; set; }
    }
}
