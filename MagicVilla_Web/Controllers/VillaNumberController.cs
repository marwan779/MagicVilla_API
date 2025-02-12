using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MagicVilla_Utility;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {

        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _mapper = mapper;
            _villaNumberService = villaNumberService;
            _villaService = villaService;

        }


        [HttpGet]
        public async Task<IActionResult> VillaNumberIndex()
        {
            List<VillaNumberDTO> Villas = new List<VillaNumberDTO>();

            APIResponse Response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            if (Response != null && Response.IsSuccess == true)
            {
                Villas = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(Response.Result));
            }

            return View(Villas);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> CreateVillaNumber()
        {
            CreateVillaNumberVM Model = new CreateVillaNumberVM();
            APIResponse Response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
                Convert.ToString(Response.Result)).
                Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });

            return View(Model);
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateVillaNumber(CreateVillaNumberVM Model)
        {
            if (ModelState.IsValid)
            {
                APIResponse Response = await _villaNumberService.CreateAsync<APIResponse>(Model.CreateVillaNumberDTO, HttpContext.Session.GetString(StaticData.SessionToken));

                if (Response != null && Response.IsSuccess == true)
                {
                    TempData["Success"] = "Villa Number Created Successfully";
                    return RedirectToAction(nameof(VillaNumberIndex));
                }
                else
                {
                    if (Response.IsSuccess == false || Response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", Response.ErrorMessages.FirstOrDefault());
                    }
                }
            }


            APIResponse GETRresponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
                Convert.ToString(GETRresponse.Result))
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });



            return View(Model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            UpdateVillaNumberVM Model = new UpdateVillaNumberVM();

            APIResponse Response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(StaticData.SessionToken));

            if(Response  != null && Response.IsSuccess == true)
            {
                VillaNumberDTO Result = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(Response.Result));
                Model.UpdateVillaNumberDTO = _mapper.Map<UpdateVillaNumberDTO>(Result);
            }

            APIResponse GETRresponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            if(GETRresponse != null && GETRresponse.IsSuccess == true)
            {
                Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
                Convert.ToString(GETRresponse.Result))
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
                return View(Model);
            }
            

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(UpdateVillaNumberVM Model)
        {
            if (ModelState.IsValid)
            {
                APIResponse Response = await _villaNumberService.UpdateAsync<APIResponse>(Model.UpdateVillaNumberDTO, HttpContext.Session.GetString(StaticData.SessionToken));

                if (Response != null && Response.IsSuccess == true)
                {
                    TempData["Success"] = "Villa Number Updated Successfully";
                    return RedirectToAction(nameof(VillaNumberIndex));
                }
                else
                {
                    if (Response.IsSuccess == false || Response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", Response.ErrorMessages.FirstOrDefault());
                    }
                }
            }


            APIResponse GETRresponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
                Convert.ToString(GETRresponse.Result))
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });



            return View(Model);
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            DeleteVillaNumberVM Model = new DeleteVillaNumberVM();
            APIResponse Response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(StaticData.SessionToken));

            if(Response != null && Response.IsSuccess == true)
            {
                VillaNumberDTO Result = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(Response.Result));
                Model.VillaNumberDTO = Result;
            }


            APIResponse GETRresponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

            if (GETRresponse != null && GETRresponse.IsSuccess == true)
            {
                Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
                Convert.ToString(GETRresponse.Result))
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
                return View(Model);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> DeleteVillaNumber(DeleteVillaNumberVM Model)
		{
			if (ModelState.IsValid)
			{
				APIResponse Response = await _villaNumberService.DeleteAsync<APIResponse>(Model.VillaNumberDTO.VillaNo, HttpContext.Session.GetString(StaticData.SessionToken));

				if (Response != null && Response.IsSuccess == true)
				{
                    TempData["Success"] = "Villa Number Deleted Successfully";
                    return RedirectToAction(nameof(VillaNumberIndex));
				}
				else
				{
					if (Response.IsSuccess == false || Response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessages", Response.ErrorMessages.FirstOrDefault());
					}
				}
			}


			APIResponse GETRresponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(StaticData.SessionToken));

			Model.VillasList = JsonConvert.DeserializeObject<List<VillaDTO>>(
				Convert.ToString(GETRresponse.Result))
				.Select(s => new SelectListItem
				{
					Text = s.Name,
					Value = s.Id.ToString()
				});



			return View(Model);
		}


	}
}
