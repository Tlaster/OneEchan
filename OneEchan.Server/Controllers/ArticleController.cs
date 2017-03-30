using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Controllers.Interface;
using OneEchan.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Controllers
{
    public class ArticleController : Controller, IPostController
    {
        private ApplicationDbContext _context;

        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IActionResult> Details(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
