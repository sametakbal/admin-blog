using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminBlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace AdminBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly BlogContext _context;

        public BlogController(ILogger<BlogController> logger, BlogContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _context.Category.Select(w =>
                new SelectListItem{
                    Text = w.Name ,
                    Value = w.Id.ToString()
                }
            ).ToList();
            return View();
        }
        public async Task<IActionResult> Save(Blog model){
            if(model != null){
                var file = Request.Form.Files.First();
                string savePath = Path.Combine("C:","Users","samet","Youtube","MyBlog","wwwroot","img");
                var fileName = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}";
                var fileUrl = Path.Combine(savePath,fileName);
                using(var fileStream = new FileStream(fileUrl,FileMode.Create)){
                    await file.CopyToAsync(fileStream);
                }
                model.ImagePath= fileName;
                model.AuthorId =(int)HttpContext.Session.GetInt32("id");
                await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                return Json(true);

            }

            return Json(false);
        }

   

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
