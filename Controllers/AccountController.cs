using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using ProgLibrary.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    [Authorize("HasUserRole")]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IBrokerService _brokerService;

        private readonly IMapper _mapper;
        public AccountController(IBrokerService brokerService, IMapper mapper)
        {
            _brokerService = brokerService;

            _mapper = mapper;
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> Index()
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<IEnumerable<AccountDto>>("Account/Browse");
            return View(_mapper.Map<IEnumerable<AccountViewModel>>(response));

        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> Delete(Guid? userId)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<AccountDto>($"Account/GetById/{userId}");
            return View(_mapper.Map<AccountViewModel>(response));
        }


        [HttpPost, ActionName("Delete")]
        [Route("[Action]")]
        public async Task<IActionResult> DeleteConfirmed(Guid userId)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.DeleteAsync($"Account/Delete/{userId}");
            ViewBag.Result = response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction(nameof(Index));
                
            }
            return RedirectToAction(nameof(Delete),userId);
            
        }

        // GET: Account/Details/{userId}
        [HttpGet("Details/{userId}")]
        [Route("[Action]")]
        public async Task<IActionResult> Details(Guid userId)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<AccountDetailsDto>($"Account/GetById/{userId}");
            return View(_mapper.Map<AccountDetailsViewModel>(response));
        }


        //[AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
