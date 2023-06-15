using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string notify)
        {
            string userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var user = await _userManager.FindByEmailAsync(userName);

            var roles = await _userManager.GetRolesAsync(user);
            var bills = _context.Bills.Where(_ => _.UserId == user.Id);

            ViewData["role"] = roles.ToArray();
            ViewData["bills"] = bills.ToArray();
            ViewData["notify"] = notify;
            return View(user);
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
					return RedirectToAction("Index", "Home");

				ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác");
            }

            model.Password = string.Empty;
            return View(model);
        }

        [Route("logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid || model.NewPassword != model.ConfirmPassword)
                return RedirectToAction(nameof(Index), new { notify = "Thông tin đổi mật khẩu không chính xác, thử lại sau" });

			string userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

			var user = await _userManager.FindByEmailAsync(userName);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index), new { notify = "Thay đổi mật khẩu thành công" });

            return RedirectToAction(nameof(Index), new { notify = "Mật khẩu cũ không chính xác" });
		}

		[HttpGet("register")]
		public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.RePassword)
                    ModelState.AddModelError(string.Empty, "Mật khẩu nhắc lại không chính xác");

                if(ModelState.ErrorCount == 0)
                {
                    var user = new IdentityUser()
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        EmailConfirmed = true,
                        PhoneNumber = model.PhoneNumber
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if(result.Succeeded)
						return RedirectToAction(nameof(Login));

					ModelState.AddModelError(string.Empty, "Thông tin đăng ký không chính xác, vui lòng kiểm tra lại");
				}    
            }

            model.Password = string.Empty;
            model.RePassword = string.Empty;
            return View();
        }
	}
}
