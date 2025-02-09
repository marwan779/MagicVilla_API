using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;

        public VillaAPIController(IMapper Mapper, IVillaRepository villaRepository)
        {
            _mapper = Mapper;
            _villaRepository = villaRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> Villas = await _villaRepository.GetAllAsync();

            return Ok(_mapper.Map<List<VillaDTO>>(Villas));
        }
    }
}
