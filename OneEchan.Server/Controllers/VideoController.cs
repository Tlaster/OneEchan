using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    public class VideoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VideoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Details(int id)
        {
            return "Hello World";
        }
    }
}
