using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using static MagicVilla_Utility.StaticData;
using System.Net.Http.Headers;
using MagicVilla_Web.Models.DTO;
using AutoMapper.Internal;
using MagicVilla_Utility;
using System.Text.Unicode;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }
        public IHttpClientFactory HttpClientFactory { get; set; }
        private readonly ITokenProvider _tokenProvider;
        private readonly string ClientURI = string.Empty;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IApiRequestMessageBuilder apiRequestMessageBuilder;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, 
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor, 
            IApiRequestMessageBuilder apiRequestMessageBuilder)
        {
            this.HttpClientFactory = httpClientFactory;
            this.ResponseModel = new APIResponse();
            _tokenProvider = tokenProvider;
            ClientURI = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            this.httpContextAccessor = httpContextAccessor;
            this.apiRequestMessageBuilder = apiRequestMessageBuilder;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool WithBearer = true)
        {
            try
            {
                var Client = HttpClientFactory.CreateClient("MagicAPI");

                var MessageFactory = () =>
                {

                    return apiRequestMessageBuilder.Buid(apiRequest);
                };

                HttpResponseMessage httpResponseMessage = null;

                httpResponseMessage = await SendWithRefreshTokenAsync(MessageFactory, Client, WithBearer);

                APIResponse FinalApiResponse = new APIResponse()
                {
                    IsSuccess = false,
                };

                try
                {

                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Not Found" };
                            break;
                        case HttpStatusCode.Forbidden:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Access Denied" };
                            break;
                        case HttpStatusCode.Unauthorized:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Unauthorized" };
                            break;
                        case HttpStatusCode.InternalServerError:
                            FinalApiResponse.ErrorMessages = new List<string>() { "Internal Server Error" };
                            break;
                        default:
                            var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                            FinalApiResponse.IsSuccess = true;
                            FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                            break;
                    }
                }
                catch (Exception e)
                {
                    FinalApiResponse.ErrorMessages = new List<string>() { "Error Encountered", e.Message.ToString() };
                }
                
                var res = JsonConvert.SerializeObject(FinalApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;

            }
            catch(AuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var Result = new APIResponse()
                {
                    ErrorMessages = new List<string>() { ex.Message.ToString() },
                    IsSuccess = false
                };

                var res = JsonConvert.SerializeObject(Result);
                var aPIResponse = JsonConvert.DeserializeObject<T>(res);

                return aPIResponse;
            }
        }

        private async Task<HttpResponseMessage> SendWithRefreshTokenAsync
            (Func<HttpRequestMessage> HttpRequestMessageFactory, HttpClient Client, bool WithBearer)
        {
            if (!WithBearer)
            {
                return await Client.SendAsync(HttpRequestMessageFactory());
            }
            else
            {
                LogInResponseDTO logInResponseDTO = _tokenProvider.GetToken();
                if (logInResponseDTO != null && !string.IsNullOrEmpty(logInResponseDTO.AccessToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", logInResponseDTO.AccessToken);
                }

                try
                {
                    HttpResponseMessage Response = await Client.SendAsync(HttpRequestMessageFactory());
                    if (Response.IsSuccessStatusCode)
                    {
                        return Response;
                    }

                    if (!Response.IsSuccessStatusCode && Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await InvokeRefreshEndPoint(Client, logInResponseDTO.AccessToken, logInResponseDTO.RefreshToken);
                        Response = await Client.SendAsync(HttpRequestMessageFactory());

                        return Response;
                    }

                    return Response;

                }
                catch(AuthException)
                {
                    throw;
                }
                catch (HttpRequestException ex)
                {

                    if (ex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await InvokeRefreshEndPoint(Client, logInResponseDTO.AccessToken, logInResponseDTO.RefreshToken);
                        return await Client.SendAsync(HttpRequestMessageFactory());

                    }


                    HttpResponseMessage Response = await Client.SendAsync(HttpRequestMessageFactory());

                    return Response;
                }

            }
        }


        private async Task InvokeRefreshEndPoint(HttpClient Client, string ExistingAccessToken, string ExistingRefreshToken)
        {
            HttpRequestMessage Message = new HttpRequestMessage();
            Message.Headers.Add("Accept", "application/json");
            Message.RequestUri = new Uri($"{ClientURI}/api/{StaticData.VillaAPIVersion}/refresh");
            Message.Method = HttpMethod.Post;

            Message.Content = new StringContent(JsonConvert.SerializeObject(new LogInResponseDTO
            {
                AccessToken = ExistingAccessToken,
                RefreshToken = ExistingRefreshToken
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage Response = await Client.SendAsync(Message);

            string Content = await Response.Content.ReadAsStringAsync();

            APIResponse apiResponse = JsonConvert.DeserializeObject<APIResponse>(Content);

            if (!apiResponse.IsSuccess)
            {
                await httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();

                throw new AuthException();
            }
            else
            {
                string ApiResponseResult = JsonConvert.SerializeObject(apiResponse.Result);
                LogInResponseDTO Tokens = JsonConvert.DeserializeObject<LogInResponseDTO>(ApiResponseResult);

                if (Tokens != null && !string.IsNullOrEmpty(Tokens.AccessToken))
                {

                    await SignInWithNewToken(Tokens);
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.AccessToken);
                }
            }

        }

        private async Task SignInWithNewToken(LogInResponseDTO logInResponseDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(logInResponseDTO.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, logInResponseDTO.LocalUser.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(c => c.Type == "role").Value));
            var principal = new ClaimsPrincipal(identity);
            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            _tokenProvider.SetToken(logInResponseDTO);
        }
    }
}
