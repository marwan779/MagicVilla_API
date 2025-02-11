using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;

        public VillaAPIController(IMapper Mapper, IVillaRepository villaRepository)
        {
            _mapper = Mapper;
            _villaRepository = villaRepository;
            this._response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {

            try
            {
                IEnumerable<Villa> Villas = await _villaRepository.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaDTO>>(Villas);
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

        [HttpGet("{Id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                Villa Result = await _villaRepository.GetAsync(v => v.Id == Id);

                if (Result == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<VillaDTO>(Result);
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] CreateVillaDTO createVillaDTO)
        {
            try
            {
                if (createVillaDTO == null)
                {
                    return BadRequest();
                }

                if (await _villaRepository.GetAsync(v => v.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
                {
					ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
					_response.IsSuccess = false;
					return BadRequest(_response);
                }

                Villa villa = _mapper.Map<Villa>(createVillaDTO);

                villa.CreatedDate = DateTime.Now;

                await _villaRepository.CreateAsync(villa);
                await _villaRepository.SaveAsync();



                _response.Result = _mapper.Map<VillaDTO>(villa);
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

        [HttpDelete("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                Villa Result = await _villaRepository.GetAsync(v => v.Id == Id);

                if (Result == null)
                {
					ModelState.AddModelError("ErrorMessages", "Villa does not Exists!");
					_response.IsSuccess = false;
					return BadRequest(_response);
                }

                await _villaRepository.DeleteAsync(Result);
                await _villaRepository.SaveAsync();

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


        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int Id, [FromBody] UpdateVillaDTO updateVillaDTO)
        {
            try
            {
                if (updateVillaDTO == null || Id != updateVillaDTO.Id)
                {
					ModelState.AddModelError("ErrorMessages", "No Villa With this Id Exists!");
					_response.IsSuccess = false;
					return BadRequest();
                }

                Villa Result = _mapper.Map<Villa>(updateVillaDTO);

                await _villaRepository.UpdateAsync(Result);
                await _villaRepository.SaveAsync();

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
