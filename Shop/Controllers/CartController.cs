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
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [Route("/cart")]
        public IActionResult Index()
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carts = _context.Carts.Include(_ => _.Product)
                .Where(_ => userId == _.UserId).OrderByDescending(_ => _.Id).ToArray();
            return View(carts);
        }

        [Authorize]
        [Route("/cart/add")]
        public IActionResult Add(Guid productId, int quantity = 1)
        {
            if (!_context.Products.Any(_ => _.Id == productId))
                return RedirectToAction("Index", "Home");

            string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = _context.Carts.FirstOrDefault(_ => _.ProductId == productId && _.UserId == userId);

            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = userId,
                    Quantity = quantity
                };
                _context.Carts.Add(cart);
            }
            else
            {
                cart.Quantity += quantity;
                if (cart.Quantity > 0)
                    _context.Entry(cart).State = EntityState.Modified;
                else
                    _context.Carts.Remove(cart);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

		[Authorize]
		[Route("/cart/change")]
		public IActionResult Change(Guid id, int quantity = 1)
		{
			var cart = _context.Carts.FirstOrDefault(_ => _.Id == id);

            if (cart == null)
                return RedirectToAction(nameof(Index));

			cart.Quantity = quantity;
			if (cart.Quantity > 0)
				_context.Entry(cart).State = EntityState.Modified;
			else
				_context.Carts.Remove(cart);
			_context.SaveChanges();

			return RedirectToAction(nameof(Index));
		}

		[Authorize]
        [Route("/cart/delete/{id}")]
        public IActionResult Remove(Guid id)
        {
            var cart = _context.Carts.FirstOrDefault(_ => _.Id == id);
            if (cart == null)
                return RedirectToAction(nameof(Index));

            _context.Carts.Remove(cart);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
