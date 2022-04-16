using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inlamningsuppgift.Services
{
    public interface IAuthenticationManager
    {
        Task<IActionResult> SignIn(SignInForm form);
        Task<IActionResult> SignUp(SignUpForm form);
    }



    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationManager(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> SignUp (SignUpForm form)
        {
            try
            {
                if (await _context.Users.AnyAsync(x => x.Email == form.Email))
                    return new ConflictObjectResult("A user with the same E-Mail already exists.");

                var newUser = new UserEntity
                {
                    Email = form.Email,
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    Address = form.Address,
                    City = form.City,
                    PostalCode = form.PostalCode,
                };

                newUser.CreateSecurePassword(form.Password);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return new OkObjectResult($"{newUser.FirstName} {newUser.LastName} created successfully.");

            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }

        public async Task<IActionResult> SignIn (SignInForm form)
        {
            if (string.IsNullOrEmpty(form.Email) || string.IsNullOrEmpty(form.Password)) return new BadRequestObjectResult("You must provide Email and Password.");

            var userEntity = await _context.Users.FirstOrDefaultAsync(x => x.Email == form.Email);
            if (userEntity == null)
                return new BadRequestObjectResult("Incorrect email or password");

            if (!userEntity.CompareSecurePassword(form.Password))
                return new BadRequestObjectResult("Incorrect email or password");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userEntity.Email),
                    new Claim(ClaimTypes.Email, userEntity.Email),
                    new Claim("UserId", userEntity.UserId.ToString()),
                    new Claim("ApiKey", _configuration.GetValue<string>("ApiKey"))
                }),

                Expires = DateTime.Now.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey"))),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return new OkObjectResult(accessToken);

        }
    }
}
