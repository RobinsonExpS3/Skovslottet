using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared
{
    public sealed class ShiftBoardViewDto
    {
        public Guid ShiftBoardId { get; set; }
        public string ShiftType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public List<ResidentCardViewDto> ResidentCards { get; set; } = new();
        public List<string> PhoneNumbers { get; set; } = new();
        public List<string> DepartmentTasks { get; set; } = new();
        public List<string> SpecialResponsibilities { get; set; } = new();
    }
}
