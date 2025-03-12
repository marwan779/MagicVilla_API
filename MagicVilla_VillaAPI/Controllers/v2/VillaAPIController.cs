using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;

        public VillaAPIController(IMapper Mapper, IVillaRepository villaRepository)
        {
            _mapper = Mapper;
            _villaRepository = villaRepository;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ResponseCache(CacheProfileName = "Default30")]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "OccupancyFilter")] int? OccupancyFilter,
            [FromQuery] string? Search, int PageSize = 0, int PageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> Villas;

                if (OccupancyFilter != null)
                {
                    Villas = await _villaRepository.GetAllAsync(v => v.Occupancy == OccupancyFilter,
                        PageSize: PageSize, PageNumber: PageNumber);
                }
                else
                {
                    Villas = await _villaRepository.GetAllAsync(PageSize: PageSize, PageNumber: PageNumber);
                }

                if (!String.IsNullOrEmpty(Search))
                {
                    Villas = await _villaRepository.GetAllAsync(v => v.Name.ToLower() == Search.ToLower());
                }

                Pagination pagination = new()
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                Villa Result = await _villaRepository.GetAsync(v => v.Id == Id);

                if (Result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] CreateVillaDTO createVillaDTO)
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

                if(createVillaDTO.Image != null)
                {
                    string FileName = villa.Id.ToString() + Path.GetExtension(createVillaDTO.Image.FileName);
                    string FilePath = @"wwwroot\ProductImage\" + FileName;
                    var DirectoryLocation = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    FileInfo File = new FileInfo(FilePath);

                    if (File.Exists)
                    {
                        File.Delete();
                    }

                    using (var fileStream = new FileStream(DirectoryLocation, FileMode.Create))
                    {
                        createVillaDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/ProductImage/" + FileName;
                    villa.ImageLocalPath = FilePath;

                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400";
                }

                await _villaRepository.UpdateAsync(villa);

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

        [Authorize(Roles = "admin")]
        [HttpDelete("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "admin")]
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


                if (!String.IsNullOrEmpty(Result.ImageUrl))
                {
                    string OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), Result.ImageLocalPath);

                    FileInfo File = new FileInfo(OldFilePath);

                    if (File.Exists)
                    {
                        File.Delete();
                    }
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

        [Authorize(Roles = "admin")]
        [HttpPut("{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int Id, [FromForm] UpdateVillaDTO updateVillaDTO)
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


                if (updateVillaDTO.Image != null)
                {

                    if (!String.IsNullOrEmpty(updateVillaDTO.ImageUrl))
                    {
                        string OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), Result.ImageUrl);

                        FileInfo File = new FileInfo(OldFilePath);

                        if (File.Exists)
                        {
                            File.Delete();
                        }
                    }

                    string FileName = Result.Id.ToString() + Path.GetExtension(updateVillaDTO.Image.FileName);
                    string FilePath = @"wwwroot\ProductImage\" + FileName;
                    var DirectoryLocation = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    

                    using (var fileStream = new FileStream(DirectoryLocation, FileMode.Create))
                    {
                        updateVillaDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    Result.ImageUrl = baseUrl + "/ProductImage/" + FileName;
                    Result.ImageLocalPath = FilePath;

                }
                else
                {
                    Result.ImageUrl = "https://placehold.co/600x400";
                }


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
