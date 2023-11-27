using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.Data;

namespace Shop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

			services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddSignInManager<SignInManager<IdentityUser>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/login";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!dbContext.Users.Any())
                {
                    var user = new IdentityUser()
                    {
                        UserName = "admin@yopmail.com",
                        Email = "admin@yopmail.com",
                        EmailConfirmed = true
                    };
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    if (userManager.CreateAsync(user, "1q2w3E*").Result.Succeeded)
                    {
                        var role = new IdentityRole() { Name = "admin" };
                        dbContext.Roles.Add(role);
                        
                        dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
                    }
					var user1 = new IdentityUser()
					{
						UserName = "admin@gmail.com",
						Email = "admin@gmail.com",
						EmailConfirmed = true
					};
					var userManager1 = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
					if (userManager1.CreateAsync(user1, "Taohanmay446@").Result.Succeeded)
					{
						var role1 = new IdentityRole() { Name = "admin" };
						dbContext.Roles.Add(role1);

						dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role1.Id, UserId = user1.Id });
					}
                }
                dbContext.SaveChanges();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

			app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
		}
    }
}
