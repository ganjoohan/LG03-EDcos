using EDocSys.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
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