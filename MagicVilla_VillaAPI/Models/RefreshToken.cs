using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string JwtTokenId { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
        public bool IsVaild { get; set; }

        public DateTime ExpiresAt {  get; set; }
    }
}
