using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService :  IVillaNumberService
    {
        private readonly IHttpClientFactory Client;
        private string CLientUrl;
        private readonly IBaseService _baseService;

        public VillaNumberService(IHttpClientFactory httpClientFactory, IConfiguration Configuration, IBaseService baseService )
        {
            Client = httpClientFactory;
            CLientUrl = Configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }
        public async Task<T> CreateAsync<T>(CreateVillaNumberDTO DTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI",
                Data = DTO,
            });

        }

        public async Task<T> DeleteAsync<T>(int Id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.DELETE,
                Url = CLientUrl+ $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + Id,
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI",
            });
        }

        public async Task<T> GetAsync<T>(int Id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + Id,
            });
        }

        public async Task<T> UpdateAsync<T>(UpdateVillaNumberDTO DTO )
        {
            return await _baseService.SendAsync<T>(new APIRequest() 
            { 
                apiType = ApiType.PUT,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + DTO.VillaNo,
                Data = DTO ,
            });
        }
    }
}
