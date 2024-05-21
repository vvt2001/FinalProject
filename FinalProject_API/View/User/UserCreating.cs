using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.User
{
    public class UserCreating
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string name { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        public string username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string password { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        public string confirm_password { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        public string email { get; set; }

    }
}
