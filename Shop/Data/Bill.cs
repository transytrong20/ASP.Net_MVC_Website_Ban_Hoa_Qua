using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Data
{
    public class Bill
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ToAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Note { get; set; }

        [Required]
        public int Status { get; set; } //0 = vừa tạo, 1 = đang làm, 2 = đang ship, 3 = thành công, 4 = thất bại, 5 = Bị hủy

        public DateTime CreationTime { get; set; }

        public virtual IEnumerable<BillDetailt> Detailts { get; set; }
    }

    public class BillDetailt
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid BillId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(BillId))]
        public Bill Bill { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
