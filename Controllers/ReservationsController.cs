using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgLibrary.Infrastructure.Commands.Reservations;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using ProgLibrary.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IBrokerService _brokerService;
        private readonly IMapper _mapper;

        public ReservationsController(IBrokerService brokerService, IMapper mapper)
        {

            _brokerService = brokerService;
            _mapper = mapper;
        }

        #region Admin
        /// <summary>
        /// This Action will show all Reservations
        /// </summary>
        /// <returns>View with mapped Dto to ViewModel</returns>
        [HttpGet]
        [Authorize("HasUserRole")] //zmienic na admin
        public async Task<IActionResult> Index()
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<IEnumerable<ReservationDto>>($"Reservations/GetReservations");
            return View(_mapper.Map<IEnumerable<ReservationViewModel>>(response));
        }

        /// <summary>
        /// This Action will make a reservation
        /// </summary>
        /// <returns>Return View </returns>
        [HttpGet]
        [Authorize("HasAdminRole")]
        [Route("[Action]")]
        public async Task<IActionResult> Create()
        {
            var client = await _brokerService.Create(HttpContext);

            var users = client.GetFromJsonAsync<IEnumerable<AccountDto>>($"Administration/GetUsers").Result.ToList();
            ViewBag.UserEmails = new SelectList(users, "Id", "Email", users.Select(u => u.Id));

            var books = await client.GetFromJsonAsync<IEnumerable<BookDto>>($"Books/Get");
            ViewBag.Books = new SelectList(books, "Id", "Title", books.Select(b => b.Id));

            return View();
        }


        #endregion
        #region User

        [HttpPost]
        [Authorize("HasUserRole")]
        [Route("[Action]")]
        public async Task<IActionResult> Create(CreateReservation command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.PostAsJsonAsync("Reservations/Create", command);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Authorize("HasUserRole")]
        [Route("[Action]")]
        public async Task<IActionResult> Details(Guid id)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<ReservationDto>($"Reservations/{id}");
            return View(_mapper.Map<ReservationViewModel>(response));
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
