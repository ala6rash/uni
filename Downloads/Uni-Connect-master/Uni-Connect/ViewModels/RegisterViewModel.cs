using System.ComponentModel.DataAnnotations;

namespace Uni_Connect.ViewModels
{
    /// <summary>
    /// ViewModel for the student registration form.
    /// All validation rules for each field are defined here using Data Annotations.
    /// </summary>
    public class RegisterViewModel
    {
        // ── Full Name ──────────────────────────────────────────────────────────
        [Required(ErrorMessage = "Please enter your full name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        // ── University Email ───────────────────────────────────────────────────
        // Must match the exact pattern: 9 digits + @philadelphia.edu.jo
        // Example: 202210882@philadelphia.edu.jo
        [Required(ErrorMessage = "Please enter your university email")]
        [RegularExpression(
            @"^\d{9}@philadelphia\.edu\.jo$",
            ErrorMessage = "Email must be exactly: 9-digit ID @philadelphia.edu.jo  (e.g. 202210882@philadelphia.edu.jo)")]
        [Display(Name = "University Email")]
        public string Email { get; set; }

        // ── Faculty ────────────────────────────────────────────────────────────
        [Required(ErrorMessage = "Please select your faculty")]
        [Display(Name = "Faculty")]
        public string Faculty { get; set; }

        // ── Year of Study ──────────────────────────────────────────────────────
        [Required(ErrorMessage = "Please select your year of study")]
        [Display(Name = "Year of Study")]
        public string YearOfStudy { get; set; }

        // ── Password ───────────────────────────────────────────────────────────
        // Minimum 8 characters with uppercase, lowercase, digit, and special char
        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, a number, and a special character (e.g. Test@1234)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // ── Confirm Password ───────────────────────────────────────────────────
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
