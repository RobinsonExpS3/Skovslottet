using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Slottet.Application.Interfaces
{
    public interface IDTOService : IController
    {
        async Task<ActionResult<IEnumerable<T>>> Getall();


    }
}
