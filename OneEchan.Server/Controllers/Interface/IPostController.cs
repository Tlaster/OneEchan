using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers.Interface
{
    public interface IPostController
    {
        Task<IActionResult> Edit(int id);
        Task<IActionResult> Details(int id);
    }
}
