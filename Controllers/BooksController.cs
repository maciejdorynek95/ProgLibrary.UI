using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    [Route("[controller]")]
    public class BooksController : Controller
    {

        private readonly IBrokerService _brokerService;

        private readonly IMapper _mapper;
        public BooksController(IBrokerService brokerService, IMapper mapper)
        {
            _brokerService = brokerService;

            _mapper = mapper;
        }
        [HttpGet]
        [Route("Show")]
        public async Task<IActionResult> Index()
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Get", "");
            var books = await response.Content.ReadFromJsonAsync<IEnumerable<BookDto>>();
            return View(_mapper.Map<IEnumerable<BookViewModel>>(books));

        }
        [HttpGet("Details")]
        public async Task<IActionResult> Details(Guid id)
        {

            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Get/bookId", id);
            var book = await response.Content.ReadFromJsonAsync<BookDetailsDto>();
            return View(_mapper.Map<BookDetailsViewModel>(book));
        }
        [HttpGet]
        [Authorize("HasAdminRole")]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpGet("Create/{command}")]
        [Authorize("HasAdminRole")]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> Create(CreateBook command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Create", command);
            if (!response.IsSuccessStatusCode)
            {
                return View("Error", command);
            }
            ViewBag.CreatedBook = command;
            return View();

        }

        [HttpGet("Edit")]
        [Authorize("HasAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Get", id);
          
            if (!response.IsSuccessStatusCode)
            {
                return View("Error", id);
            }
            var book = await response.Content.ReadFromJsonAsync<BookDto>();
            return View(_mapper.Map<BookViewModel>(book));
        }

        [Authorize("HasAdminRole")]
        [ValidateAntiForgeryToken]
        public  IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        [Authorize("HasAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Edit/bookId", id);
            var book = await response.Content.ReadFromJsonAsync<BookDetailsDto>();
            return View(_mapper.Map<BookDetailsViewModel>(book));

        }

        [HttpDelete]
        [Authorize("HasAdminRole")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Books/Delete/bookId", id);
            var book = await response.Content.ReadFromJsonAsync<BookDetailsDto>();

            try
            {
                RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View();
            }

            return Ok();

        }


    }
}
