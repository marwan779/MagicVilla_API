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
        }

        public string GetToken()
        {
            String TokenResult = string.Empty;

            try
            {
               bool HasAccessToken =  _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticData.AccessToken, out TokenResult);

                return TokenResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public void SetToken(string Token)
        {
            CookieOptions cookieOptions = new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddDays(60)
            };
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.AccessToken, Token, cookieOptions);
        }
    }
}
