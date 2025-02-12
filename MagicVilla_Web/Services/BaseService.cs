using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using static MagicVilla_Utility.StaticData;
using System.Net.Http.Headers;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }
        public IHttpClientFactory HttpClientFactory { get; set; }

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory;
            this.ResponseModel = new APIResponse();
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var Client = HttpClientFactory.CreateClient("MagicAPI");
                HttpRequestMessage Message = new HttpRequestMessage();
                Message.Headers.Add("Accept", "application/json");
                Message.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data != null)
                {
                    Message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        System.Text.Encoding.UTF8, "application/json");
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
