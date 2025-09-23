using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var updateduser = user.Adapt<UpdatePersonalInfoResponseDTO>();

            return Ok(updateduser);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateInfo(UpdatePersonalInfoRequestDTO personalInfoDTO)
        {

            var user = await _userManager.FindByIdAsync(personalInfoDTO.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            user.Email = personalInfoDTO.Email;
            user.FirstName = personalInfoDTO.FirstName;
            user.LastName = personalInfoDTO.LastName;
            user.PhoneNumber = personalInfoDTO.PhoneNumber;
            user.Addresse = personalInfoDTO.Addresse;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }


        [HttpPatch("ChangePhoto")]
        public async Task<IActionResult> ChangePhoto(ChangePhotoRequest changePhotoRequest)
        {
            var user = await _userManager.FindByIdAsync(changePhotoRequest.ApplicationUserId);

            if (user is null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }

            if (changePhotoRequest.Photo is not null && changePhotoRequest.Photo.Length > 0)
            {
                var oldpath = "";

                if (user.ProfileImage is not null || user.ProfileImage == "")
                {
                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\profile", user.ProfileImage);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(changePhotoRequest.Photo.FileName);
                var pathName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\profile", fileName);
                using (var stream = System.IO.File.Create(pathName))
                {
                    await changePhotoRequest.Photo.CopyToAsync(stream);
                }

                user.ProfileImage = fileName;
                await _userManager.UpdateAsync(user);

                //remove old photo ==> using the old path

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }

                return NoContent();

            }

            return BadRequest(new ResponseErrorDTO
            {
                Msg = "Failed to change profile image",
                TraceId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow

            });

        }



        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangeProfilePasswordDTO changeProfilePasswordDTO)
        {

            var user = await _userManager.FindByIdAsync(changeProfilePasswordDTO.ApplicationUserId);
            if (user == null)
            {
                return NotFound(new ResponseErrorDTO
                {
                    Msg = "Invalid User name Or Email",
                    TraceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow

                });
            }
            var result = await _userManager.ChangePasswordAsync(user, changeProfilePasswordDTO.CurrentPassword, changeProfilePasswordDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors

                });
            }

            await _userManager.UpdateAsync(user);

            return NoContent();
        }


    }
}
