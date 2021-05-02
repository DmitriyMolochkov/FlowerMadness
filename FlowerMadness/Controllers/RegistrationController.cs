using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Core.Interfaces;
using DAL.Models;
using FlowerMadness.Authorization;
using FlowerMadness.Helpers;
using FlowerMadness.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlowerMadness.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<RegistrationController> _logger;
        private const string GetCurrentUserActionName = "GetCurrentUser";
        private const string GetRoleByIdActionName = "GetRoleById1";

        public RegistrationController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService,
            ILogger<RegistrationController> logger)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _logger = logger;
        }
        
        [HttpPost("users")]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register([FromBody] UserCreateModel user)
        {
            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);

                var result = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
                if (result.Succeeded)
                {
                    UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                    return CreatedAtAction(GetCurrentUserActionName, userVM);
                }

                AddError(result.Errors);
            }

            return BadRequest(ModelState);
        }


        [HttpGet("users/me")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var id = Utilities.GetUserId(this.User);
            
            UserViewModel userVM = await GetUserViewModelHelper(id);

            if (userVM != null)
                return Ok(userVM);
            else
                return NotFound(id);
        }

        [HttpPut("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateModel user)
        {
            var id = Utilities.GetUserId(this.User);

            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                if (appUser == null)
                    return NotFound(id);

                bool isPasswordChanged = !string.IsNullOrWhiteSpace(user.NewPassword);
                bool isUserNameChanged = !appUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

                if (Utilities.GetUserId(this.User) == id)
                {
                    if (string.IsNullOrWhiteSpace(user.CurrentPassword))
                    {
                        if (isPasswordChanged)
                            AddError("Current password is required when changing your own password", "Password");

                        if (isUserNameChanged)
                            AddError("Current password is required when changing your own username", "Username");
                    }
                    else if (isPasswordChanged || isUserNameChanged)
                    {
                        if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                            AddError("The username/password couple is invalid.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _mapper.Map<UserUpdateModel, ApplicationUser>(user, appUser);

                    var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);
                    if (result.Succeeded)
                    {
                        if (isPasswordChanged)
                        {
                            if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                                result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                            else
                                result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                        }

                        if (result.Succeeded)
                            return NoContent();
                    }

                    AddError(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }
        
        private async Task<UserViewModel> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserViewModel>(userAndRoles.Value.User);
            userVM.Roles = userAndRoles.Value.Roles;

            return userVM;
        }

        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }

        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            if (role != null)
                return _mapper.Map<RoleViewModel>(role);


            return null;
        }
    }
}
