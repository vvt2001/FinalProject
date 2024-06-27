using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.User
{
    public class UserUpdating
    {
        [Required(ErrorMessage = "User not found")]
        public string id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }
    }
}
