using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Data;
using cloudscribe.Web.Pagination;
using cloudscribe.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace OneEchan.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 20;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Video")]
        public IActionResult Video(int page = 0)
        {
            return View(_context.Video.Include(v => v.VideoUrl).Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).ToPagedList(page, PAGE_SIZE));
        }

        [Route("Article")]
        public IActionResult Article(int page = 0)
        {
            return View(_context.Article.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).ToPagedList(page, PAGE_SIZE));
        }

        [Route("Search")]
        public IActionResult Search(string query, int page = 0)
        {
            return View(_context.Post.Where(item => item.Title.Contains(query) && item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).ToPagedList(page, PAGE_SIZE));
        }

        [Route("AllContent")]
        public IActionResult AllContent(int page = 0)
        {
            return View(_context.Post.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).ToPagedList(page, PAGE_SIZE));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
