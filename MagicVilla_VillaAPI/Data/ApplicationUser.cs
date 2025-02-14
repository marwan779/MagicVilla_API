using Microsoft.AspNetCore.Identity;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
