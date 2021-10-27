using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.User;
using ProgLibrary.Infrastructure.DTO;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProgLibrary.UI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IBrokerService _brokerService;
        public AccountController(IBrokerService brokerService)
        {
            _brokerService = brokerService;
        }
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoginController/Details/5
        [HttpGet]
        public ActionResult Details(Guid userId)
        {
            return View();
        }

        // GET: LoginController/Create
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> SignIn(Login command)
        {
            var client = await _brokerService.Create(HttpContext);
            var response = await _brokerService.SendJsonAsync(client,"Account/login", command);
           
            var jwt = await response.Content.ReadFromJsonAsync<TokenDto>();
            Response.HttpContext.Session.SetString("Token", jwt.Token);

            return Ok($"Zalogowano użytkownika: {command.Email}");

      
  
        }

    }
}
