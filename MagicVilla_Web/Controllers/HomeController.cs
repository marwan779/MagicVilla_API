using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVilla_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public HomeController(IVillaService villaService, IMapper mapper)
        {
            _mapper = mapper;
            _villaService = villaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> Villas = new List<VillaDTO>();
            APIResponse Response = await _villaService.GetAllAsync<APIResponse>();
            if (Response != null && Response.IsSuccess == true)
            {
                Villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(Response.Result));
            }

            return View(Villas);
        }
        
    }
}
