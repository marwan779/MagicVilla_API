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
        public Task<T> CreateAsync<T>(CreateVillaDTO DTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + "/api/villaAPI",
                Data = DTO
            });

        }

        public Task<T> DeleteAsync<T>(int Id)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.DELETE,
                Url = CLientUrl+"/api/villaAPI"+Id,
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + "/api/villaAPI",
            });
        }

        public Task<T> GetAsync<T>(int Id)
        {
            return SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.GET,
                Url = CLientUrl + "/api/villaAPI"+Id,
            });
        }

        public Task<T> UpdateAsync<T>(UpdateVillaDTO DTO)
        {
            return SendAsync<T>(new APIRequest() 
            { 
                apiType = ApiType.PUT,
                Url = CLientUrl + "/api/villaAPI"+DTO.Id,
                Data = DTO 
            });
        }
    }
}
