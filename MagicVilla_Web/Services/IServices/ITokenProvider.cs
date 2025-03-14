using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web.Services.IServices
{
    public interface ITokenProvider
    {
        public void ClearToken();

        public LogInResponseDTO GetToken();

        public void SetToken(LogInResponseDTO logInResponseDTO);
    }
}
