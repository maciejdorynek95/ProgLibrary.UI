using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.Settings.JwtToken;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    [Route("[controller]")]
    public class BooksController : Controller
    {

        private readonly IBrokerService _brokerService;
        public BooksController(IBrokerService brokerService)
        {
            _brokerService = brokerService;

        }
        // GET: BooksController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BooksController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: BooksController/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateBook command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Create", command);
            return Ok($"Dodano: {await response.Content.ReadAsStringAsync()}");
            
        }

        

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BooksController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BooksController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
