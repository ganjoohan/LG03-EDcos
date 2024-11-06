using EDocSys.Application.Features.ActivityLog.Commands.AddLog;
using EDocSys.Infrastructure.Identity.Models;
using EDocSys.Web.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel<LoginModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IMediator _mediator;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                // Always use the full email address for authentication
                var email = Input.Email.Trim().ToLower();
                if (!IsValidEmail(email))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email format.");
                    return Page();
                }

                // Check if the email exists in the database
                var usersWithEmail = await _userManager.Users.Where(u => u.NormalizedEmail == email.ToUpper()).ToListAsync();

                if (!usersWithEmail.Any())
                {
                    ModelState.AddModelError(string.Empty, "Email not found.");
                    _notyf.Error("Email not found.");
                    return Page();
                }

                // Extract the domain from the email
                string extractedUsername = email.Split('@')[0];

                // Check if the extracted username exists in the usersWithEmail list
                var user = usersWithEmail.FirstOrDefault(u => u.UserName.Equals(extractedUsername, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    // If not found, create a new username by combining username and part of the domain
                    var emailParts = email.Split('@');
                    var domainParts = emailParts[1].Split('.');
                    string newUsername = emailParts[0] + (domainParts.Length > 3 ? (domainParts[0] + domainParts[1]) : domainParts[0]);

                    // Find the user with the new username
                    user = usersWithEmail.FirstOrDefault(u => u.UserName.Equals(newUsername, StringComparison.OrdinalIgnoreCase));
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found for this email.");
                    _notyf.Error("User not found for this email.");
                    return Page();
                }

                if (!user.IsActive)
                {
                    _notyf.Error("Email / Username Not Found.");
                    ModelState.AddModelError(string.Empty, "Email / Username Not Found.");
                    return RedirectToPage("./Deactivated");
                }

                if (!user.EmailConfirmed)
                {
                    _notyf.Error("Email Not Confirmed.");
                    ModelState.AddModelError(string.Empty, "Email Not Confirmed.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Logged In" });
                    _logger.LogInformation("User logged in.");
                    _notyf.Success($"Logged in as {user.UserName}.");
                    return LocalRedirect(returnUrl);
                }

                await _mediator.Send(new AddActivityLogCommand() { userId = user.Id, Action = "Log-In Failed" });
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _notyf.Warning("User account locked out.");
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                _notyf.Error("Invalid login attempt.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}