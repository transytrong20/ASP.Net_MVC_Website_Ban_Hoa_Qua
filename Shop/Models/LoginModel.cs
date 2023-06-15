using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class LoginModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email đăng nhập không được bỏ trống")]
        public string Email { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu đăng nhập không được bỏ trống")]
        public string Password { get; set; }
    }
}
