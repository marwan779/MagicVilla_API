using AutoMapper.Internal;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Services
{
    public class ApiRequestMessageBuilder : IApiRequestMessageBuilder
    {
        public HttpRequestMessage Buid(APIRequest Request)
        {
            HttpRequestMessage Message = new HttpRequestMessage();

            if (Request.ContentType == ContentType.MultipartFormData)
            {
                Message.Headers.Add("Accept", "*/*");
            }
            else
            {
                Message.Headers.Add("Accept", "application/json");
            }

            Message.RequestUri = new Uri(Request.Url);


            if (Request.ContentType == ContentType.MultipartFormData)
            {
                var Content = new MultipartFormDataContent();

                foreach (var Property in Request.Data.GetType().GetProperties())
                {
                    var Value = Property.GetValue(Request.Data);

                    if (Value is FormFile)
                    {
                        var File = (FormFile)Value;
                        Content.Add(new StreamContent(File.OpenReadStream()), Property.Name, File.FileName);

                    }
                    else
                    {
                        Content.Add(new StringContent(Value == null ? "" : Value.ToString()), Property.Name);
                    }
                }

                Message.Content = Content;
            }
            else
            {
                if (Request.Data != null)
                {
                    Message.Content = new StringContent(JsonConvert.SerializeObject(Request.Data),
                        System.Text.Encoding.UTF8, "application/json");
                }
            }


            switch (Request.apiType)
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

            return Message;
        }
    }
}
