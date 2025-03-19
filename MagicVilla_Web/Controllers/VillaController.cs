using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _mapper = mapper;
            _villaService = villaService;
        }

        [HttpGet]
        public async Task<IActionResult> VillaIndex()
        {
            List<VillaDTO> Villas = new List<VillaDTO>();
            APIResponse Response = await _villaService.GetAllAsync<APIResponse>();
            if (Response != null && Response.IsSuccess == true)
            {
                Villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(Response.Result));
            }

            return View(Villas);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateVilla(CreateVillaDTO Model)
        {
            if (ModelState.IsValid)
            {
                APIResponse Response = await _villaService.CreateAsync<APIResponse>(Model);
                if (Response != null && Response.IsSuccess == true)
                {
                    TempData["Success"] = "Villa Created Successfully";
                    return RedirectToAction(nameof(VillaIndex));
                }
                else
                {
                    TempData["Error"] = Response.ErrorMessages.FirstOrDefault();
                }
            }

            return View(Model);
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            APIResponse Response = await _villaService.GetAsync<APIResponse>(villaId);

            if (Response != null && Response.IsSuccess == true)
            {
                VillaDTO DTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(Response.Result));

                return View(_mapper.Map<UpdateVillaDTO>(DTO));
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateVilla(UpdateVillaDTO Model)
        {
            if (ModelState.IsValid)
            {
                APIResponse Response = await _villaService.UpdateAsync<APIResponse>(Model);
                if (Response != null && Response.IsSuccess == true)
                {
                    TempData["Success"] = "Villa Updated Successfully";
                    return RedirectToAction(nameof(VillaIndex));
                }
                else
                {
                    TempData["Error"] = Response.ErrorMessages.FirstOrDefault();
                }
            }

            return View(Model);
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            APIResponse Response = await _villaService.GetAsync<APIResponse>(villaId);

            if (Response != null && Response.IsSuccess == true)
            {
                VillaDTO DTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(Response.Result));

                return View(DTO);
            }

            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaDTO Model)
        {

            APIResponse Response = await _villaService.DeleteAsync<APIResponse>(Model.Id);
            if (Response != null && Response.IsSuccess == true)
            {
                TempData["Success"] = "Villa Deleted Successfully";
                return RedirectToAction(nameof(VillaIndex));
            }
            else
            {
                TempData["Error"] = Response.ErrorMessages.FirstOrDefault();
            }

            return View(Model);
        }

    }
}
