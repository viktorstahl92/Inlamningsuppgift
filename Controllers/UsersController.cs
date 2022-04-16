using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IUserManager _userManager;

        public UsersController(IUserManager authenticationManager)
        {
            _userManager = authenticationManager;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpForm form) => await _userManager.SignUp(form);

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInForm form) => await _userManager.SignIn(form);

        [HttpDelete("{userID}")]
        public async Task<IActionResult> DeleteUser(int userID) => await _userManager.DeleteUser(userID);

        [HttpGet("{userID}")]
        public async Task<IActionResult> GetUserById(int userID)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                // or
                Console.WriteLine(identity.Claims.Single(x => x.Type == ClaimTypes.Name)); 

            }

            return await _userManager.GetUserById(userID);
        }

        [HttpPut("{userID}")]
        public async Task<IActionResult> UpdateUser(int userID, UserInfo form) => await _userManager.UpdateUser(userID, form);
    }
}
