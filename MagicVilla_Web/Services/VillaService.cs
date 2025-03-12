using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService 
    {
        private readonly IHttpClientFactory Client;
        private string CLientUrl;

        public VillaService(IHttpClientFactory httpClientFactory, IConfiguration Configuration ): base( httpClientFactory )
        {
            Client = httpClientFactory;
            CLientUrl = Configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(CreateVillaDTO DTO , string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaAPI",
                Data = DTO,
                Token = Token,
                ContentType = StaticData.ContentType.MultipartFormData
            });

        }

        public Task<T> DeleteAsync<T>(int Id , string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.DELETE,
                Url = CLientUrl+ $"api/{StaticData.VillaAPIVersion}/VillaAPI/" + Id,
                Token = Token
            });
        }

        public Task<T> GetAllAsync<T>(string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaAPI",
                Token = Token
            });
        }

        public Task<T> GetAsync<T>(int Id, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaAPI/" + Id,
                Token = Token
            });
        }

        public Task<T> UpdateAsync<T>(UpdateVillaDTO DTO , string Token)
        {
            return SendAsync<T>(new APIRequest() 
            { 
                apiType = ApiType.PUT,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/VillaAPI/" + DTO.Id,
                Data = DTO ,
                Token = Token,
                ContentType = StaticData.ContentType.MultipartFormData
            });
        }
    }
}
