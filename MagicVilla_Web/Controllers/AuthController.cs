using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogInRequestDTO logInRequestDTO)
        {
            APIResponse Response = await _authService.LoginAsync<APIResponse>(logInRequestDTO);

            if(Response != null && Response.IsSuccess == true)
            {
                LogInResponseDTO logInResponseDTO = JsonConvert.DeserializeObject<LogInResponseDTO>(Convert.ToString(Response.Result));

                HttpContext.Session.SetString(StaticData.SessionToken, logInResponseDTO.Token);

                return RedirectToAction("Index", "Home");   
            }
            else
            {
                ModelState.AddModelError("CustomErorr", Response.ErrorMessages.FirstOrDefault());
                return View(logInRequestDTO);
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            APIResponse Response = await _authService.RegisterAsync<APIResponse>(registerationRequestDTO);
            if(Response != null && Response.IsSuccess == true) 
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(StaticData.SessionToken, "");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
