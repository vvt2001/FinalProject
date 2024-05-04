using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.User
{
    public class UserCreating
    {
        [Required(ErrorMessage = "Họ và tên không được trống")]
        public string name { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được trống")]
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }

    }
}
