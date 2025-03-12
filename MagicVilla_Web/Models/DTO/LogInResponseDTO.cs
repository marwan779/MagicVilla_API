namespace MagicVilla_Web.Models.DTO
{
    public class LogInResponseDTO
    {
        public UserDTO LocalUser { get; set; }

        public string AccessToken { get; set; }   
    }
}
