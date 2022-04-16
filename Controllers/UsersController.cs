using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetUserById(int userID) => await _userManager.GetUserById(userID);

        [HttpPut("{userID}")]
        public async Task<IActionResult> UpdateUser(int userID, UserInfo form) => await _userManager.UpdateUser(userID, form);
    }
}
