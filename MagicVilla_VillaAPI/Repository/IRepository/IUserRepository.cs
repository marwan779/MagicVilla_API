using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUnique(string username);

        Task<LogInResponseDTO> LogIn(LogInRequestDTO logInRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<LogInResponseDTO> RefreshAccessToken(LogInResponseDTO logInResponseDTO);
        Task RevokeRefreshToken(LogInResponseDTO logInResponseDTO);

    }
}
