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
using Microsoft.EntityFrameworkCore;

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

            string TokenId = $"JTI{Guid.NewGuid()}";
            string accessToken = await GenerateToken(User, TokenId);
            string refreshToken = await CreateNewRefreshToken(User.Id, TokenId);

            LogInResponseDTO Response = new LogInResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                LocalUser = _mapper.Map<UserDTO>(User),
            };


            return Response;
        }

        private async Task<string> GenerateToken(ApplicationUser User, string TokenId)
        {
            var Roles = await _userManger.GetRolesAsync(User);
            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(SecertKey);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User.UserName.ToString()),
                    new Claim(ClaimTypes.Role, Roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, TokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, User.Id)
                }),

                Expires = DateTime.UtcNow.AddMinutes(60),
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

        public async Task<LogInResponseDTO> RefreshAccessToken(LogInResponseDTO logInResponseDTO)
        {
            RefreshToken ExistingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Refresh_Token == logInResponseDTO.RefreshToken);
            if (ExistingRefreshToken == null)
            {
                return new LogInResponseDTO();
            }

            bool TokenValid = GetAccessTokenData(logInResponseDTO.AccessToken, ExistingRefreshToken.UserId, ExistingRefreshToken.JwtTokenId);
            if (!TokenValid)
            {
                await MarkTokenAsInvaild(ExistingRefreshToken);
                return new LogInResponseDTO();
            }

            if (ExistingRefreshToken.IsVaild == false)
            {
                await MarkAllTokenInChainAsInvaild(ExistingRefreshToken.JwtTokenId, ExistingRefreshToken.UserId);

                return new LogInResponseDTO();
            }


            if (ExistingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvaild(ExistingRefreshToken);
                return new LogInResponseDTO();
            }

            string NewRefreshToken = await CreateNewRefreshToken(ExistingRefreshToken.UserId, ExistingRefreshToken.JwtTokenId);


            await MarkTokenAsInvaild(ExistingRefreshToken);

            ApplicationUser AppUser = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.Id == ExistingRefreshToken.UserId);
            if (AppUser == null)
            {
                return new LogInResponseDTO();
            }

            string NewAccessToken = await GenerateToken(AppUser, ExistingRefreshToken.JwtTokenId);

            return new LogInResponseDTO()
            {
                AccessToken = NewAccessToken,
                RefreshToken = NewRefreshToken
            };


        }

        public async Task RevokeRefreshToken(LogInResponseDTO logInResponseDTO)
        {
            RefreshToken refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Refresh_Token == logInResponseDTO.RefreshToken);

            if (refreshToken == null)
            {
                return;
            }

            bool TokenValid = GetAccessTokenData(logInResponseDTO.AccessToken, refreshToken.UserId, refreshToken.JwtTokenId);
            if (!TokenValid)
            {
                return;
            }

            await MarkAllTokenInChainAsInvaild(refreshToken.JwtTokenId, refreshToken.UserId);

        }


        private async Task<string> CreateNewRefreshToken(string userId, string TokenId)
        {
            RefreshToken refreshToken = new RefreshToken()
            {
                IsVaild = true,
                UserId = userId,
                JwtTokenId = TokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Refresh_Token;

        }

        private bool GetAccessTokenData(string AccessToken, string ExepectedUserId, string ExpectedTokenId)
        {
            try
            {
                JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
                var Token = TokenHandler.ReadJwtToken(AccessToken);

                string UserId = Token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                string TokenId = Token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                return (UserId == ExepectedUserId && TokenId == ExpectedTokenId);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private async Task MarkAllTokenInChainAsInvaild(string xpectedTokenId, string ExepectedUserId)
        {
            List<RefreshToken> RefreshTokens = _context.RefreshTokens.Where(r => r.UserId == ExepectedUserId
                && r.JwtTokenId == xpectedTokenId).ToList();

            foreach (RefreshToken RefreshToken in RefreshTokens)
            {
                RefreshToken.IsVaild = false;
            }

            _context.UpdateRange(RefreshTokens);
            _context.SaveChanges();

        }

        private Task MarkTokenAsInvaild(RefreshToken refreshToken)
        {
            refreshToken.IsVaild = false;
            return _context.SaveChangesAsync();
        }

    }
}

