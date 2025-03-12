using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private readonly IHttpClientFactory Client;
        private string CLientUrl;

        public VillaNumberService(IHttpClientFactory httpClientFactory, IConfiguration Configuration ): base( httpClientFactory )
        {
            Client = httpClientFactory;
            CLientUrl = Configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(CreateVillaNumberDTO DTO, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI",
                Data = DTO,
                Token = Token
            });

        }

        public Task<T> DeleteAsync<T>(int Id, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.DELETE,
                Url = CLientUrl+ $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + Id,
                Token = Token
            });
        }

        public Task<T> GetAllAsync<T>(string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI",
                Token = Token
            });
        }

        public Task<T> GetAsync<T>(int Id , string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + Id,
                Token = Token
            });
        }

        public Task<T> UpdateAsync<T>(UpdateVillaNumberDTO DTO , string Token)
        {
            return SendAsync<T>(new APIRequest() 
            { 
                apiType = ApiType.PUT,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaNumberAPI/" + DTO.VillaNo,
                Data = DTO ,
                Token = Token
            });
        }
    }
}
