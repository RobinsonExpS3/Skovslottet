using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Shared
{
    public sealed class PnScheduleItemDto
    {
        public string Label { get; set; } = "PN";
        public string TimeLabel { get; set; } = string.Empty; 
        public bool IsGiven { get; set; }                     
    }
}
