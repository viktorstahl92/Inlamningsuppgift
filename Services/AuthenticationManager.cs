using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Services
{
    public interface IAuthenticationManager
    {
        Task<IActionResult> SignUp(SignUpForm form);
    }



    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly DataContext _context;

        public AuthenticationManager(DataContext context)
        {
            _context = context;
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
    }
}
