using Homework03._11.Data;
using HomeWork03._11.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace HomeWork03._11.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=ImagesHomework;Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Image(int id)
        {
            Manager mgr = new(_connectionString);
            ImagesViewModel vm = new();
            vm.Image = mgr.GetImage(id);
            if (HttpContext.Session.Get<List<int>>("viewed") != null)
            {
                vm.ViewedIds = HttpContext.Session.Get<List<int>>("viewed");
                if (vm.ViewedIds.Contains(id))
                {
                    mgr.AddView(id);
                }
            }
            vm.id = id;
            

            return View(vm);
        }

        [HttpPost]
        public IActionResult Uploaded(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";

            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            var mgr = new Manager(_connectionString);
            ImageClass item = new()
            {
                Password = password,
                ImagePath = fullImagePath
            };
            mgr.Add(item);

            ImagesViewModel vm = new();
            vm.Image = item;
            return View(vm);
        }
        
        [HttpPost]
        public IActionResult Image(int id, string password)
        {
            Manager mgr = new(_connectionString);
            if(!mgr.CorrectPassword(id, password))
            {
                TempData["message"] = "Invalid password";
                return Redirect($"/home/images?{id}");
            }

            List<int> ids = HttpContext.Session.Get<List<int>>("viewed");
            if (ids == null)
            {
                ids = new();
            }
            ids.Add(id);
            HttpContext.Session.Set("viewed", ids);
            mgr.AddView(id);

            return Redirect($"/home/images?{id}");
        }

    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
