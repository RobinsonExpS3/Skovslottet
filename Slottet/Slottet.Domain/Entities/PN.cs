using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Domain.Entities
{
    public class PN 
    {
        public Guid PNID { get; set; }
        public DateTime PNTime { get; set; }
        public string PNStatus { get; set; }

        public Guid ResidentStatusID { get; set; }
        public ResidentStatus ResidentStatus { get; set; }
    }
}
