using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.User
{
    public class UserCreating
    {
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        public string confirm_password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

    }
}
