using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.User
{
    public class UserUpdating
    {
        [Required(ErrorMessage = "Không xác định được người dùng")]
        public string id { get; set; }
        [Required(ErrorMessage = "Họ và tên không được trống")]
        public string name { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được trống")]
        public string email { get; set; }
    }
}
