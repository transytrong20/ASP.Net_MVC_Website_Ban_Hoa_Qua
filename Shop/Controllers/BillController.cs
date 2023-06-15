using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Controllers
{
	public class BillController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BillController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		[Authorize]
		[HttpPost("/bill/checkout")]
		public IActionResult Checkout(Bill bill)
		{
			string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

			var carts = _context.Carts.Include(_ => _.Product)
				.Where(_ => _.UserId == userId).ToArray();

			bill.Id = Guid.NewGuid();
			bill.UserId = userId;
			bill.CreationTime = DateTime.Now;
			bill.Status = 0;

			if (string.IsNullOrEmpty(bill.PhoneNumber))
			{
				var current = _context.Users.FirstOrDefault(_ => _.Id == userId);
				bill.PhoneNumber = current?.PhoneNumber;
			}

			_context.Add(bill);
			var details = carts.Select(_ => new BillDetailt()
			{
				ProductId = _.ProductId,
				BillId = bill.Id,
				Price = _.Product.Price,
				Quantity = _.Quantity,
				Id = Guid.NewGuid()
			});
			_context.Detailts.AddRange(details);
			_context.Carts.RemoveRange(carts);
			_context.SaveChanges();

			return RedirectToAction("Index", "Account", new { notify = "Đặt hàng thành công" });
		}

		[Route("/bill/view/{id}")]
		public IActionResult ViewDetailt(Guid id)
		{
			var bill = _context.Bills
				.Include(_ => _.Detailts)
				.ThenInclude(_ => _.Product)
				.FirstOrDefault(_ => _.Id == id);
			if (bill == null)
				return RedirectToAction("Index", "Account", new { notify = "Không tìm thấy thông tin hóa đơn" });

			return View(bill);
		}

		[Route("/bill/cancle/{id}")]
		public IActionResult Cancle(Guid id)
		{
			var bill = _context.Bills
				.FirstOrDefault(_ => _.Id == id);

			if (bill == null)
				return RedirectToAction("Index", "Account", new { notify = "Không tìm thấy thông tin hóa đơn" });

			if (bill.Status != 0)
				return RedirectToAction("Index", "Account", new { notify = "Hóa đơn không thể bị hủy" });

			bill.Status = 5;
			_context.Entry(bill).State = EntityState.Modified;
			_context.SaveChanges();

			return RedirectToAction("Index", "Account", new { notify = "Hủy hóa đơn thành công" });
		}

		[Route("/manager/bill")]
		public IActionResult Manager(string keyword, string notify)
		{
			var bills = _context.Bills.Include(_ => _.Detailts).AsQueryable();

			if (!string.IsNullOrEmpty(keyword))
				bills = bills.Where(_ => _.ToAddress.ToLower().Contains(keyword.ToLower()) || _.PhoneNumber.ToLower().Contains(keyword.ToLower()));

			bills = bills.OrderBy(_ => _.Status).ThenBy(_ => _.CreationTime);

			return View(bills.ToArray());
		}

		[Route("/manager/bill/change")]
		public IActionResult Change(Guid id, int status)
		{
			var bill = _context.Bills.FirstOrDefault(_ => _.Id == id);
			if (bill == null)
				return RedirectToAction(nameof(Manager), new { notify = "Không tìm thấy thông tin hóa đơn" });

			bill.Status = status;
			_context.Entry(bill).State = EntityState.Modified;
			_context.SaveChanges();

			return RedirectToAction(nameof(Manager), new { notify = "Không tìm thấy thông tin hóa đơn" });
		}

	}
}
