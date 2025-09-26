using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<UserOTP> _userOTP;

        public AccountsController(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            IRepository<UserOTP> userOTP)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userOTP = userOTP;
        }






        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                Addresse = registerDTO.Addresse
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            //  add role user => customer

            await _userManager.AddToRoleAsync(user, SD.CustomerRole);

            //confirm message 

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confim your Email", $"<h1> Confirm your  email by clicking  <a target='_blank' href='{link}'>Here</a> </h1>");


            return Ok(new
            {
                msg = "User created Successfully,check your inbox to confirm"
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {

            var user = await _userManager.FindByEmailAsync(loginDTO.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginDTO.EmailOrUserName);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {

                    Msg = "Invalid User name Or password",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            //_userManager.CheckPasswordAsync();
            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return BadRequest(new ResponseErrorDTO
                    {

                        Msg = "Too Many Attemps",
                        TraceId = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow

                    });
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest(new ResponseErrorDTO
                {

                    Msg = "Confirm Your Email First!",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            if (!user.LockoutEnabled)
            {
                return BadRequest(new ResponseErrorDTO
                {
                    Msg = $"You have a block till {user.LockoutEnd}",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            return Ok(new
            {
                msg = "Login successfully"

            });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound(new
                {
                    msg = "Can not find this user"
                });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest(new ResponseErrorDTO
                {
                    Msg = "Link Expired!, Resend Email Confirmation",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }


            else
            {
                return Ok(new
                {
                    msg = "Confirm Email successfully"
                });
            }

        }

        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationDTO resendEmailConfirmationDTO)
        {


            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationDTO.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationDTO.EmailOrUserName);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new ResponseErrorDTO
                {
                    Msg = "Already Confirmed!",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            // Send confirmation msg
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, $"Confirm Your Email!", $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            return Ok(new
            {
                msg = "Send Email successfully, Confirm Your Email!"
            });
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {


            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.EmailOrUserName) ?? await _userManager.FindByNameAsync(forgetPasswordDTO.EmailOrUserName);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            // Send confirmation msg
            var OTPNumber = new Random().Next(1000, 9999);
            var link = Url.Action("ResetPassword", "Account", new { area = "Identity", userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, $"Reset Password!", $"<h1>Reset Password Using {OTPNumber}. Don't share it!</h1>");

            await _userOTP.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                OTPNumber = OTPNumber,
                ValidTo = DateTime.UtcNow.AddDays(1)
            });
            await _userOTP.CommitAsync();

            return Ok(new
            {
                userId = user.Id,
                msg = "Send OTP Number to Your Email successfully"

            });
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {


            var user = await _userManager.FindByIdAsync(resetPasswordDTO.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            var userOTP = (await _userOTP.GetAsync(e => e.ApplicationUserId == resetPasswordDTO.ApplicationUserId)).OrderBy(e => e.Id).LastOrDefault();

            if (userOTP is null)
                return NotFound();

            if (userOTP.OTPNumber != resetPasswordDTO.OTPNumber)
            {
                return BadRequest(new ResponseErrorDTO
                {
                    Msg = "Invalid OTP",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            if (DateTime.UtcNow > userOTP.ValidTo)
            {
                return BadRequest(new ResponseErrorDTO
                {
                    Msg = "Expired OTP",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }
            return Ok(new
            {

                msg = "Success OTP",
                UserId = user.Id
            });
        }

        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordDTO newPasswordDTO)
        {


            var user = await _userManager.FindByIdAsync(newPasswordDTO.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPasswordDTO.Password);

            return Ok(new
            {
                msg = "Change Password Successfully!"
            });
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new
            {
                msg = "Logout Successfully"
            });
        }
    }
}
