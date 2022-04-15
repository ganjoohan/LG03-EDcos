using System.ComponentModel.DataAnnotations;

namespace EDocSys.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}