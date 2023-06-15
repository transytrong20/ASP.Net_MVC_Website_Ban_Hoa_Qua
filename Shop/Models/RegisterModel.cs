using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Email không được bỏ trống")]
		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Email không chính xác")]
		public string Email { get; set; }
		[Display(Name = "Số điện thoại")]
		[Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
		public string PhoneNumber { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
		[Display(Name = "Mật khẩu")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Mật khẩu phải dài từ 6 ký tự và phải chứa ít nhất 1 chữ cái thường, 1 chữ in hoa, 1 ký tự đặc biệt và 1 chữ số")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Mật khẩu nhắc lại không được bỏ trống")]
		[Display(Name = "Mật khẩu nhắc lại")]
		public string RePassword { get; set; }
	}
}
