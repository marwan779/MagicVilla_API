using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices
{
    public interface IApiRequestMessageBuilder
    {
        HttpRequestMessage Buid(APIRequest Request);
    }
}
