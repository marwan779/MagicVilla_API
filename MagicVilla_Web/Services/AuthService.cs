using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Services
{
    public class AuthService: IAuthService
    {
        private readonly IHttpClientFactory Client;
        private string CLientUrl;
        private readonly IBaseService _baseService;
        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration Configuration, IBaseService baseService)
        {
            Client = httpClientFactory;
            CLientUrl = Configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> LoginAsync<T>(LogInRequestDTO logInRequestDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/UserAuth/Login",
                Data = logInRequestDTO
            }, WithBearer: false);
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                apiType = ApiType.POST,
                Url = CLientUrl + $"api/{StaticData.VillaAPIVersion}/UserAuth/Register",
                Data = registerationRequestDTO
            }, WithBearer: false);
        }
    }
}
