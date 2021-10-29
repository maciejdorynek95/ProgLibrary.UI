using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.ViewModels;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    [AllowAnonymous]
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
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoginController/Details/Guid
        [HttpGet("Details/{userId}")]
        public async Task<IActionResult> Details(Guid userId)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client, "Account/GetById", userId);
            var user = await response.Content.ReadFromJsonAsync<AccountDto>();
            return View(_mapper.Map<AccountViewModel>(user));
        }

 


    }
}
