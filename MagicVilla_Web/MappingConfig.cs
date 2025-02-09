using AutoMapper;
using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDTO, CreateVillaDTO>().ReverseMap();
            CreateMap<VillaDTO, UpdateVillaDTO>().ReverseMap();

            CreateMap<VillaNumberDTO, CreateVillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, UpdateVillaNumberDTO>().ReverseMap();
        }
    }
}
