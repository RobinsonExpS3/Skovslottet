using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities {
    public class PaymentMethod {
        public Guid PaymentMethodID { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
