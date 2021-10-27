using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    //[Route("[controller]")]
    public class BooksController : Controller
    {

        private readonly IBrokerService _brokerService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBrokerService brokerService, IBookService bookService, IMapper mapper)
        {
            _brokerService = brokerService;
            _bookService = bookService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.BrowseAsync();
            return View(_mapper.Map<IEnumerable<BookViewModel>>(books));
        }


        public async Task<IActionResult> Details(Guid id)
        {
             var book = await _bookService.GetAsync(id);
            return View(_mapper.Map<BookDetailsViewModel>(book));
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBook command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Create", command);
            if (!response.IsSuccessStatusCode)
            {
                return View("Error", command);
            }
            ViewBag.Book = command;
            return View();

        }






        public IActionResult Edit(Guid id)
        {
            return View(_bookService.GetAsync(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, IFormCollection collection)
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


        public async Task<IActionResult> Delete(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return View();
        }


    }
}
