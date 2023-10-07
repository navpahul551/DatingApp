using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _uow;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow)
        {
            _userManager = userManager;    
            _uow = uow;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new 
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("Please select atleast one role!");

            var selectedRoles = roles.Split(',').ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if(user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded) return BadRequest("Failed to add roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded) return BadRequest("Failed to remove from roles!");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration() 
        {
            var unapprovedPhotos = await _uow.PhotoRepository.GetUnApprovedPhotosAsync();

            return Ok(unapprovedPhotos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _uow.PhotoRepository.GetPhotoByIdAsync(photoId);

            if(photo == null) return NotFound("Photo not found!");
            if(photo.IsApproved == true) return BadRequest("This photo is already approved!");

            // approve the photo
            photo.IsApproved = true;

            // set the photo as the main photo of the user if they don't have a main photo yet
            var user = await _uow.UserRepository.GetUserByIdAsync(photo.AppUserId);

            if(!user.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            if(await _uow.Complete()) return Ok();

            return BadRequest("Failed to update photo approval status!");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _uow.PhotoRepository.GetPhotoByIdAsync(photoId);

            if(photo == null) return NotFound("Photo not found!");

            _uow.PhotoRepository.DeletePhoto(photo);

            if(await _uow.Complete()) return Ok();

            return BadRequest("Failed to reject photo!");
        }
    }
}