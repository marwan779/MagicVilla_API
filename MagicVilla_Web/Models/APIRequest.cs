using static MagicVilla_Utility.StaticData;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public ApiType apiType {  get; set; } = ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object Data {  get; set; }
        public string Token { get; set; } 
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
