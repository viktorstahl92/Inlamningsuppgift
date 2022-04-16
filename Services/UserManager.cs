using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inlamningsuppgift.Services
{
    public interface IUserManager
    {
        Task<IActionResult> DeleteUser(int userId);
        Task<UserEntity> GetUserByEmail(string email);
        Task<IActionResult> GetUserById(int userId);
        Task<IActionResult> SignIn(SignInForm form);
        Task<IActionResult> SignUp(SignUpForm form);
        Task<IActionResult> UpdateUser(int userId, UserInfo form);
    }



    public class UserManager : IUserManager
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserManager(DataContext context, IConfiguration configuration)
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

        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult("No user with specified User-ID found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _context.Users.FindAsync(userId);


            if (user == null)
            {
                return new NotFoundObjectResult("No user with specified User-ID found");
            }

            return new OkObjectResult(new UserInfo
            {
                Address = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserId = userId,
            });
        }


        //TODO: Make Null Check better
        public async Task<UserEntity> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<IActionResult> UpdateUser(int userId, UserInfo form)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult("No user with specified User-ID found");
            }

            user.Address = form.Address;
            user.City = form.City;
            user.PostalCode = form.PostalCode;
            user.Email = form.Email;
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new OkObjectResult(user);
        }
    }
}
