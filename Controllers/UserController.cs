using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserManager _authenticationManager;

        public UserController(IUserManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpForm form) => await _authenticationManager.SignUp(form);

        [HttpPost("SignIn")]
        public async Task<IActionResult> SingIn(SignInForm form) => await _authenticationManager.SignIn(form);
    }
}
