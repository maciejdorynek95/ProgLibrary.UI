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
        public async Task<IActionResult> Index()
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await client.GetFromJsonAsync<IEnumerable<AccountDto>>("Account/Browse");
            return View(_mapper.Map<IEnumerable<AccountViewModel>> (response));
        
        }

        // GET: LoginController/Details/Guid
        [HttpGet("Details/{userId}")]
        public async Task<IActionResult> Details(Guid userId)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonGetAsync(client, "Account/GetById", userId);
            var user = await response.Content.ReadFromJsonAsync<AccountDto>();
            return View(_mapper.Map<AccountViewModel>(user));
        }


        //[AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
