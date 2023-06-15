using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Shop.Models
{
	public class ChangePasswordModel
	{
		[Display(Name = "Mật khẩu cũ")]
		[Required(ErrorMessage = "Mật khẩu cũ không được bỏ trống")]
		public string CurrentPassword { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
		[Display(Name = "Mật khẩu")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Mật khẩu phải dài từ 6 ký tự và phải chứa ít nhất 1 chữ cái thường, 1 chữ in hoa, 1 ký tự đặc biệt và 1 chữ số")]
		public string NewPassword { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
		[Display(Name = "Mật khẩu nhắc lại")]
		public string ConfirmPassword { get; set; }
    }
}
