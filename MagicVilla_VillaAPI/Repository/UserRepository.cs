using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private string SecertKey;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration,
             UserManager<ApplicationUser> userManger, IMapper mapper, RoleManager<IdentityRole> roleManager,
              SignInManager<ApplicationUser> signInManager)
        {
            _userManger = userManger;
            _mapper = mapper;
            _context = context;
            SecertKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
            _signInManager = signInManager;

        }
        public bool IsUnique(string username)
        {
            ApplicationUser User = _context.ApplicationUsers
                 .FirstOrDefault(u => u.UserName == username);

            if (User == null)
            {
                return true;
            }

            return false;


            throw new NotImplementedException();
        }

        public async Task<LogInResponseDTO> LogIn(LogInRequestDTO logInRequestDTO)
        {
            ApplicationUser User = _context.ApplicationUsers
               .FirstOrDefault(l => l.UserName.ToLower() == logInRequestDTO.UserName.ToLower());

            bool IsVaild = await _userManger.CheckPasswordAsync(User, logInRequestDTO.Password);

            if (User == null || IsVaild == false)
            {
                return new LogInResponseDTO
                {
                    AccessToken = "",
                    LocalUser = null,
                };
            }

            string accessToken = await GenerateToken(User);

            LogInResponseDTO Response = new LogInResponseDTO
            {
                AccessToken = accessToken,
                LocalUser = _mapper.Map<UserDTO>(User),
            };


            return Response;
        }

        public async Task<string> GenerateToken(ApplicationUser User)
        {
            var Roles = await _userManger.GetRolesAsync(User);
            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(SecertKey);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User.UserName.ToString()),
                    new Claim(ClaimTypes.Role, Roles.FirstOrDefault())
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            };

            var Token1 = TokenHandler.CreateToken(TokenDescriptor);

            string Result = TokenHandler.WriteToken(Token1);

            return Result;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser User = new()
            {
                UserName = registerationRequestDTO.UserName,
                Name = registerationRequestDTO.Name,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.UserName.ToUpper()
            };

            try
            {
                var result = await _userManger.CreateAsync(User, registerationRequestDTO.Password);

                if (result.Succeeded)
                {

                    if (!_roleManager.RoleExistsAsync(registerationRequestDTO.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerationRequestDTO.Role));                        
                    }

                    await _userManger.AddToRoleAsync(User, registerationRequestDTO.Role);
                    ApplicationUser UserToReturn = _context.ApplicationUsers
                        .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);


                    await _signInManager.SignInAsync(User, isPersistent: false);
                    return _mapper.Map<UserDTO>(UserToReturn);
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                return new UserDTO();
            }

        }
    }
}

