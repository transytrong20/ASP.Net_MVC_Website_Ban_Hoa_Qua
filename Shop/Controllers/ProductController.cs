using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("/product")]
        public IActionResult Index(string keyword)
        {
            var products = _context.Products.AsNoTracking();
            if (!string.IsNullOrEmpty(keyword))
                products = products.Where(_ => _.Name.ToLower().Contains(keyword.ToLower()));

            products = products.OrderByDescending(_ => _.Id);
            ViewData["keyword"] = keyword;
            return View(products.ToArray());
        }

        [Route("/product/view/{id}")]
        public IActionResult ViewDetailt(Guid id)
        {
            var product = _context.Products
                .AsNoTracking().FirstOrDefault(_ => _.Id == id);

            if (product == null)
                return RedirectToAction(nameof(Index));

            var products = _context.Products
                .AsNoTracking().Where(_ => _.Id != id)
                .OrderByDescending(_ => _.Id).Take(3).ToArray();

            ViewData["products"] = products;
            return View(product);
        }

        [Route("/manager/product")]
        public IActionResult Manager(string keyword, string notify)
        {
            var products = _context.Products.AsNoTracking();
            if (!string.IsNullOrEmpty(keyword))
                products = products.Where(_ => _.Name.ToLower().Contains(keyword.ToLower()));

            products = products.OrderByDescending(_ => _.Id);
            ViewData["keyword"] = keyword;
            ViewData["notify"] = notify;
            return View(products.ToArray());
        }

        [Route("/manager/product/add")]
        public IActionResult Add()
        {
            return View(new Product());
        }

        [HttpPost("/manager/product/add")]
        public IActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.UploadFile == null)
                {
                    ModelState.AddModelError(nameof(Product.UploadFile), "Ảnh sản phẩm không được bỏ trống");
                    return View(product);
                }

                product.Id = Guid.NewGuid();
                string webRootPath = _webHostEnvironment.WebRootPath;
                string filePath = Path.Combine("images", "products", $"{product.Id}_{product.UploadFile.FileName}");
                using (var stream = new FileStream(Path.Combine(webRootPath, filePath), FileMode.Create))
                {
                    product.UploadFile.CopyTo(stream);
                }
                product.Image = $"/{filePath}";

                try
                {
                    _context.Products.Add(product);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Manager), new { notify = "Thêm mới sản phẩm thành công" });
                }
                catch (Exception)
                {
                    if (System.IO.File.Exists(Path.Combine(webRootPath, filePath)))
                        System.IO.File.Delete(Path.Combine(webRootPath, filePath));

                    return RedirectToAction(nameof(Manager), new { notify = "Thêm mới sản phẩm không thành công" });
                }
            }

            return View(product);
        }

        [Route("/manager/product/edit/{id}")]
        public IActionResult Edit(Guid id)
        {
            var product = _context.Products.FirstOrDefault(_ => _.Id == id);
            if(product == null)
                return RedirectToAction(nameof(Manager), new { notify = "Không tìm thấy thông tin sản phẩm" });

            return View(product);
        }

        [HttpPost("/manager/product/edit")]
        public IActionResult Edit(Product model)
        {

            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(_ => _.Id == model.Id);
                if (product == null)
                    return RedirectToAction(nameof(Manager), new { notify = "Không tìm thấy thông tin sản phẩm" });

                product.Name = model.Name;
                product.Price = model.Price;
                product.Description = model.Description;

                if(model.UploadFile != null)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    using (var stream = new FileStream(Path.Combine(webRootPath, product.Image.Replace("/images", "images")), FileMode.Create))
                    {
                        model.UploadFile.CopyTo(stream);
                    }
                }
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction(nameof(Manager), new {notify = "Cập nhập thông tin sản phẩm thành công"});
            }

            return View(model);
        }

        [Route("/manager/product/delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var product = _context.Products.FirstOrDefault(_ => _.Id == id);
            if (product == null)
                return RedirectToAction(nameof(Manager), new { notify = "Không tìm thấy sản phẩm" });

            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(product.Image))
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string filePath = Path.Combine(webRootPath, product.Image.Replace("/images", "images"));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                return RedirectToAction(nameof(Manager), new { notify = "Xóa sản phẩm thành công" });
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Manager), new { notify = "Sản phẩm tạm thời không thể bị xóa" });
            }
        }
    }
}
