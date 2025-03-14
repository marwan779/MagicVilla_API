using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using static MagicVilla_Utility.StaticData;
using System.Net.Http.Headers;
using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }
        public IHttpClientFactory HttpClientFactory { get; set; }
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            this.HttpClientFactory = httpClientFactory;
            this.ResponseModel = new APIResponse();
            _tokenProvider = tokenProvider;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool WithBearer = true)
        {
            try
            {
                var Client = HttpClientFactory.CreateClient("MagicAPI");

                HttpRequestMessage Message = new HttpRequestMessage();

                if (apiRequest.ContentType == ContentType.MultipartFormData) 
                {
                    Message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    Message.Headers.Add("Accept", "application/json");
                }

                Message.RequestUri = new Uri(apiRequest.Url);

                if(WithBearer && _tokenProvider.GetType() != null)
                {
                    LogInResponseDTO logInDTO = _tokenProvider.GetToken();
                    
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", logInDTO.AccessToken);
                }

                if(apiRequest.ContentType == ContentType.MultipartFormData)
                {
                    var Content = new MultipartFormDataContent();

                    foreach(var Property in apiRequest.Data.GetType().GetProperties())
                    {
                        var Value = Property.GetValue(apiRequest.Data);

                        if(Value is FormFile)
                        {
                            var File = (FormFile)Value;
                            Content.Add(new StreamContent(File.OpenReadStream()), Property.Name, File.FileName);

                        }
                        else
                        {
                            Content.Add(new StringContent(Value == null ? "": Value.ToString()), Property.Name);
                        }
                    }

                    Message.Content = Content;
                }
                else
                {
                    if (apiRequest.Data != null)
                    {
                        Message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                            System.Text.Encoding.UTF8, "application/json");
                    }
                }


                switch (apiRequest.apiType)
                {
                    case ApiType.POST:
                        Message.Method = HttpMethod.Post;
                        break;

                    case ApiType.GET:
                        Message.Method = HttpMethod.Get;
                        break;

                    case ApiType.PUT:
                        Message.Method = HttpMethod.Put;
                        break;

                    case ApiType.DELETE:
                        Message.Method = HttpMethod.Delete;
                        break;
                }

                HttpResponseMessage apiResponse = null;

                if(!String.IsNullOrEmpty(apiRequest.Token))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }

				apiResponse = await Client.SendAsync(Message);

				var apiContent = await apiResponse.Content.ReadAsStringAsync();
				try
				{
					APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
					if (ApiResponse != null && (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest
						|| apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound))
					{
						ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
						ApiResponse.IsSuccess = false;
						var res = JsonConvert.SerializeObject(ApiResponse);
						var returnObj = JsonConvert.DeserializeObject<T>(res);
						return returnObj;
					}
				}
				catch (Exception e)
				{
					var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
					return exceptionResponse;
				}
				var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
				return APIResponse;

			}
            catch (Exception ex)
            {
                var Result = new APIResponse()
                {
                    ErrorMessages = new List<string>() { ex.Message },
                    IsSuccess = false
                };

                var res = JsonConvert.SerializeObject(Result);
                var aPIResponse = JsonConvert.DeserializeObject<T>(res);

                return aPIResponse;
            }
        }
    }
}
