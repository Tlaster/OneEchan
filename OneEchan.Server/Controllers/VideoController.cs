using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Controllers.Interface;
using OneEchan.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    public class VideoController : Controller, IPostController
    {
        private readonly ApplicationDbContext _context;

        public VideoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IActionResult> Details(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var video = await _context.Video.FindAsync(id);
            return View(video);
        }
    }
}
