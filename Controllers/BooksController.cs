using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using ProgLibrary.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
        [Authorize("HasUserRole")]
        public async Task<IActionResult> Index()
        {
            var client = await _brokerService.Create(HttpContext);
            var response2 = await client.GetFromJsonAsync<IEnumerable<BookDto>>("Books/Get");
            return View(_mapper.Map<IEnumerable<BookViewModel>>(response2));
        }


        [HttpGet("{id}")]
        [Route("[Action]")]
        [Authorize("HasUserRole")]
        public async Task<IActionResult> Details(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response2 = await client.GetFromJsonAsync<BookDetailsDto>($"Books/Get/{id}");
            return View(_mapper.Map<BookDetailsViewModel>(response2));
        }


        [HttpGet]
        [Route("[Action]")]
        [Authorize("HasAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("[Action]")]
        [Authorize("HasAdminRole")]
        public async Task<IActionResult> Create(CreateBook command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = Task.FromResult(await client.PostAsJsonAsync("Books/Create", command));
            ViewBag.Result = await response.Result.Content.ReadAsStringAsync();
            return View();

        }

        [HttpGet("{id}")]
        [Route("[Action]")]
        [Authorize("HasAdminRole")]

        public async Task<IActionResult> Edit(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<BookDto>($"Books/Get/{id}");
            if (response != null)
            {
                return View(_mapper.Map<BookViewModel>(response));
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        [Route("[Action]")]
        [Authorize("HasAdminRole")]

        public async Task<IActionResult> Update(Guid id,UpdateBook book )
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.PutAsJsonAsync($"Books/Update/{id}",book);
            response.EnsureSuccessStatusCode();

            try
            {
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch (Exception)
            {

                return View(nameof(Index));
            }

         

            


        }


        [HttpDelete("{id}")]
        [Route("[Action]")]
        [Authorize("HasAdminRole")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonPostAsync(client, "Books/Delete", id);
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

        //[AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
