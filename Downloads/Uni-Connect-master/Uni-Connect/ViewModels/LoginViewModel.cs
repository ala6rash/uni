using System.ComponentModel.DataAnnotations;

namespace Uni_Connect.ViewModels
{
    /// <summary>
    /// This ViewModel represents the data that the Login form sends to the server.
    /// Only 2 fields: Email and Password.
    /// </summary>
    public class LoginViewModel
    {
        // --- Student ID or University Email ---
        // Accepts: "202210882" OR "202210882@philadelphia.edu.jo"
        [Required(ErrorMessage = "Please enter your Student ID or university email")]
        [Display(Name = "Student ID or Email")]
        public string Email { get; set; }

        // --- Password field ---
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
