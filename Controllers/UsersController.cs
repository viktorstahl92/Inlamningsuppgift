using Inlamningsuppgift.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UseApiKey]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IUserManager _userManager;

        public UsersController(IUserManager authenticationManager)
        {
            _userManager = authenticationManager;
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpForm form) => await _userManager.SignUp(form);

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInForm form) => await _userManager.SignIn(form);

        [HttpDelete("{userID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int userID) => await _userManager.DeleteUser(userID);

        [HttpGet("{userID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int userID) => await _userManager.GetUserById(userID);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers() => await _userManager.GetUsers();
        
        [HttpGet("LoggedInUser")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            string userEmail = "";
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                userEmail = identity.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("User not authorized");
            }

            return new OkObjectResult(await _userManager.GetUserByEmail(userEmail));
        }



        [HttpPut("{userID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int userID, UserInfo form) => await _userManager.UpdateUser(userID, form);
    }
}
