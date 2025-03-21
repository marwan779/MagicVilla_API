﻿using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;

        public VillaNumberAPIController(IMapper Mapper, IVillaNumberRepository villaNumberRepository,
            IVillaRepository villaRepository)
        {
            _mapper = Mapper;
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {

            try
            {
                IEnumerable<VillaNumber> Villas = await _villaNumberRepository.GetAllAsync(IncludeProperties: "Villa");

                _response.Result = _mapper.Map<List<VillaNumberDTO>>(Villas);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }


            return _response;
        }

        [HttpGet("{Id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                VillaNumber Result = await _villaNumberRepository.GetAsync(v => v.VillaNo == Id, IncludeProperties: "Villa");

                if (Result == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(Result);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;

        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] CreateVillaNumberDTO createVillaNumberDTO)
        {
            try
            {
                if (createVillaNumberDTO == null)
                {
                    return BadRequest();
                }

                if (await _villaNumberRepository.GetAsync(v => v.VillaNo == createVillaNumberDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                if (await _villaRepository.GetAsync(v => v.Id == createVillaNumberDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "No Villa With this Id Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                VillaNumber villa = _mapper.Map<VillaNumber>(createVillaNumberDTO);

                villa.CreatedDate = DateTime.Now;

                await _villaNumberRepository.CreateAsync(villa);
                await _villaNumberRepository.SaveAsync();



                _response.Result = _mapper.Map<VillaNumberDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int Id)
        {
            Console.WriteLine("=========================================>Here<===================================");

            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                VillaNumber Result = await _villaNumberRepository.GetAsync(v => v.VillaNo == Id);

                if (Result == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa does not Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                await _villaNumberRepository.DeleteAsync(Result);
                await _villaNumberRepository.SaveAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int Id, [FromBody] UpdateVillaNumberDTO updateVillaNumberDTO)
        {
            try
            {
                if (updateVillaNumberDTO == null || Id != updateVillaNumberDTO.VillaNo)
                {
                    return BadRequest();
                }

                if (await _villaRepository.GetAsync(v => v.Id == updateVillaNumberDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "No Villa With this Id Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                VillaNumber Result = _mapper.Map<VillaNumber>(updateVillaNumberDTO);
                await _villaNumberRepository.UpdateAsync(Result);
                await _villaNumberRepository.SaveAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }
    }
}
