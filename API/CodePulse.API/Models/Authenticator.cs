using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Models {

    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase {

        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            ITokenRepository tokenRepository) {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost] // {apiBaseUrl}/api/authentication/login
        [Route("login")]
        public async Task<IActionResult> login([FromBody] loginRequestDTO request){
            IdentityUser? identityUser = await userManager.FindByEmailAsync(request.email);

            if (identityUser == null) {
                ModelState.AddModelError("", "EmailTokenProvider or Password is Incorrect");
                return ValidationProblem(ModelState);
            }

            bool passwordMatch = await userManager.CheckPasswordAsync(identityUser, request.password);

            if (passwordMatch == false) {
                ModelState.AddModelError("", "EmailTokenProvider or Password is Incorrect");
                return ValidationProblem(ModelState); 
            }

            // get the roles of the user
            IList<String> roles = await userManager.GetRolesAsync(identityUser);
            // create/retrieve a token for the user
            string jwtToken = tokenRepository.CreateJWTToken(identityUser, roles.ToList());

            // convert the login information to an object to hold the authentication token and user roles
            loginResponseDTO response = new loginResponseDTO() {
                email = request.email,
                token = jwtToken,
                roles = roles.ToList()
            };


            return Ok(response);
        }

        //POST: {apiBaseUrl}/api/authentication/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequestDTO registerRequest) {
            //create identity user object
            var user = new IdentityUser {
                UserName = registerRequest.email?.Trim(),
                Email = registerRequest.email?.Trim()
            };

            var identityResult = await userManager.CreateAsync(user, registerRequest.password);

            // make sure that user creation went off without a hitch
            if (!identityResult.Succeeded) {
                foreach(var error in identityResult.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
                return ValidationProblem(ModelState);
            }
            identityResult = await userManager.AddToRoleAsync(user, "Reader");

            // make sure that assigning a user the "reader" role went off without a hitch
            if (!identityResult.Succeeded) {
                foreach (var error in identityResult.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
                return ValidationProblem(ModelState);
            }

            return Ok();

        }
    }

    public class loginResponseDTO {
        public required string email { get; set; }
        public required string token { get; set; }
        public List<string> roles { get; set; }
    }

    public class loginRequestDTO {
        public required string email { get; set; }
        public required string password { get; set; }
    }

    public class RegisterRequestDTO {
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
