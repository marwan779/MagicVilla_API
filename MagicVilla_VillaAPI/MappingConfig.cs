using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig() 
        {
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, CreateVillaDTO>().ReverseMap();
            CreateMap<Villa, UpdateVillaDTO>().ReverseMap();
        }
    }
}
