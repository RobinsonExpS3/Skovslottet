using Microsoft.AspNetCore.Components;

namespace Slottet.Client.Components.AdminHeader
{
    public partial class AdminHeader
    {
        [Parameter] public string Active { get; set; } = string.Empty;
    }
}
