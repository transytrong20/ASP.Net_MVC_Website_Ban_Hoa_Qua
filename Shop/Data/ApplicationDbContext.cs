using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetailt> Detailts { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>()
                .ToTable("app_user");
            builder.Entity<IdentityRole>()
                .ToTable("app_role");
            builder.Entity<IdentityUserRole<string>>()
                .ToTable("app_user_role");
            builder.Entity<Product>()
                .ToTable("app_product");
            builder.Entity<Bill>()
                .ToTable("app_bill");
            builder.Entity<BillDetailt>()
                .ToTable("app_bill_detailt");
			builder.Entity<Cart>()
			   .ToTable("app_cart");
		}
    }
}
