using MagicVilla_Utility;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticData.AccessToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticData.RefreshToken);
        }

        public LogInResponseDTO GetToken()
        {
            String JWTTokenResult = string.Empty;
            String RefreshTokenResult = string.Empty;

            try
            {
               bool HasAccessToken =  _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticData.AccessToken, out JWTTokenResult);
               bool HasRefreshToken =  _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticData.RefreshToken, out RefreshTokenResult);


                LogInResponseDTO logInResponseDTO = new LogInResponseDTO()
                {
                    RefreshToken = RefreshTokenResult,
                    AccessToken = JWTTokenResult,
                };


                return HasAccessToken == true ? logInResponseDTO : null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public void SetToken(LogInResponseDTO logInResponseDTO)
        {
            CookieOptions cookieOptions = new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddDays(60)
            };

            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.AccessToken, logInResponseDTO.AccessToken, cookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.RefreshToken, logInResponseDTO.RefreshToken, cookieOptions);
        }
    }
}
