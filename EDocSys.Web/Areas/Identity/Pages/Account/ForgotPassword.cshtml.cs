using EDocSys.Application.DTOs.Mail;
using EDocSys.Application.Interfaces.Shared;
using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IMailService mailSender)
        {
            _userManager = userManager;
            _mailService = mailSender;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

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

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                MailRequest mail = new MailRequest()
                {
                    To = Input.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                };
                await _mailService.SendAsync(mail);
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

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