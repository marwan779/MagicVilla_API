namespace MagicVilla_VillaAPI.Models.DTO
{
    public class LogInResponseDTO
    {
        public UserDTO LocalUser { get; set; }
        public string AccessToken { get; set; }   
        public string RefreshToken { get; set; }   
    }
}
