using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string SecertKey;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            SecertKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUnique(string username)
        {
            LocalUser User = _context.LocalUsers
                 .FirstOrDefault(u => u.UserName == username);

            if(User == null)
            {
                return true;
            }

            return false;


            throw new NotImplementedException();
        }

        public async Task<LogInResponseDTO> LogIn(LogInRequestDTO logInRequestDTO)
        {
            LocalUser User = _context.LocalUsers
               .FirstOrDefault(l => l.UserName.ToLower() == logInRequestDTO.UserName.ToLower() 
               && l.Password == logInRequestDTO.Password);

            if(User == null)
            {
                return new LogInResponseDTO
                {
                    Token = "",
                    LocalUser = null,
                };
            }

            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(SecertKey);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User.Id.ToString()),
                    new Claim(ClaimTypes.Role, User.Role)
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new (new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            };

            var Token1 = TokenHandler.CreateToken(TokenDescriptor);

            LogInResponseDTO Response = new LogInResponseDTO 
            {
                Token = TokenHandler.WriteToken(Token1),
                LocalUser = User
            };


            return Response;
        }

        public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            LocalUser User = new()
            {
                Name = registerationRequestDTO.Name,
                UserName = registerationRequestDTO.UserName,
                Password = registerationRequestDTO.Password,
                Role = registerationRequestDTO.Role,
            };

            _context.Add(User);
            await _context.SaveChangesAsync();

            User.Password = "";


            return User;

            throw new NotImplementedException();
        }
    }
}
