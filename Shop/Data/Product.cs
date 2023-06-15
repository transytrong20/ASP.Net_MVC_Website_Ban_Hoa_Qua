using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shop.Data
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm không được bỏ trống")]
        [Range(1000, int.MaxValue, ErrorMessage = "Giá sản phẩm tối thiểu phải là 1000vnđ")]
        public int Price { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Mô tả sản phẩm không được bỏ trống")]
        public string Description { get; set; }

        public virtual IEnumerable<BillDetailt> Bills { get; set; }
        public virtual IEnumerable<Cart> Carts { get; set; }

        [NotMapped]
        public IFormFile UploadFile { get; set; }
    }
}
