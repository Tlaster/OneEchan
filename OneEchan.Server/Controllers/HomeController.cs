using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEchan.Server.Data;
using cloudscribe.Web.Pagination;
using cloudscribe.Core.Models;
using Microsoft.EntityFrameworkCore;
using OneEchan.Server.Models;

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
            var hotVideo = _context.Video.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.ViewCount).Take(5).ToList();
            var hotArticle = _context.Article.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.ViewCount).Take(5).ToList();
            var newVideo = _context.Video.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).Take(5).ToList();
            var newArticle = _context.Article.Where(item => item.PostState == Models.Post.State.Published).OrderByDescending(item => item.CreatedAt).Take(5).ToList();
            return View(new HomeViewModel
            {
                HotVideo = hotVideo,
                HotArticle = hotArticle,
                NewVideo = newVideo,
                NewArticle = newArticle,
            });
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
