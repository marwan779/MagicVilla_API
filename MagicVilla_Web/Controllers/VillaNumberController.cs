using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {

        private readonly IVillaNumberService _villaNumberService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper)
        {
            _mapper = mapper;
            _villaNumberService = villaNumberService;
        }


        [HttpGet]
        public async Task<IActionResult> VillaNumberIndex()
        {
            List<VillaNumberDTO> Villas = new List<VillaNumberDTO>();

            APIResponse Response = await _villaNumberService.GetAllAsync<APIResponse>();

            if (Response != null && Response.IsSuccess == true)
            {
                Villas = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(Response.Result));
            }

            return View(Villas);
        }
    }
}
