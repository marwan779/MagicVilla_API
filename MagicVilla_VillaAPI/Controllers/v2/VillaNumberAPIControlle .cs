using AutoMapper;
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
       public IEnumerable<string> GetStrings()
        {
            return new List<string>()
            {
                "marwan",
                "mohamed"
            };
        }
    }
}
