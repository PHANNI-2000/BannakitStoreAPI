using AutoMapper;
using BannakitStoreApi.Data;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;
using BannakitStoreApi.Reponsitory.IReponsitory;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BannakitStoreApi.Reponsitory
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        private string secretKey;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null)
            {
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == loginRequestDTO.Username.ToLower());
            bool isValid = user != null ? _passwordHasher.Verify(user.Password, loginRequestDTO.Password) : false;

            if (user == null || !isValid)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }

            // if user was found generate JWT Token
            var roles = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == user.RoleId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, roles.RoleNameEn.ToLower())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
            };

            return loginResponseDTO;
        }

        public async Task<UserDTO> Management(ManagementRequestDTO managementRequestDTO)
        {
            var currentDate = DateTime.Now;
            var passwordHash = _passwordHasher.Hash(managementRequestDTO.Password);

            User user = new()
            {
                Username = managementRequestDTO.Username,
                Password = passwordHash,
                RoleId = managementRequestDTO.RoleId ?? 2,
                CreatedBy = "SYSTEM",
                CreatedDate = currentDate,
                UpdatedBy = "SYSTEM",
                UpdatedDate = currentDate,
            };

            try
            {
                int result = 0;
                _context.Users.Add(user);
                result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    int userId = user.UserId;
                    Employee employee = managementRequestDTO.Adapt<Employee>();

                    employee.UserId = userId;
                    employee.CreatedBy = "SYSTEM";
                    employee.UpdatedBy = "SYSTEM";
                    employee.CreatedDate = currentDate;
                    employee.UpdatedDate = currentDate;
                    employee.DepartmentId = managementRequestDTO.DepartmentId;

                    await _context.Employees.AddAsync(employee);
                    result = _context.SaveChanges();
                    var userToReturn = _context.Users.FirstOrDefault(u => u.Username == managementRequestDTO.Username);

                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new UserDTO();
        }
    }
}
