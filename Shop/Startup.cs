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
                }
                if (!dbContext.Products.Any())
                    dbContext.Products.AddRange(GetProducts());

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

        private IEnumerable<Product> GetProducts()
        {
            return new Product[]
            {
                new Product(){ Id = Guid.NewGuid(), Name = "Bánh mì Việt Nam", Price = 30000, Image = "/images/products/banhmi.jpg", Description = "Bánh mì Việt Nam là một món ăn đặc trưng và phổ biến trong ẩm thực Việt. Bánh mì Việt Nam thường có vỏ giòn tan, bên trong mềm mịn và được làm từ bột mỳ và men mỳ tự nhiên. Nó thường được chế biến với các loại nhân đa dạng như thịt, gia vị, rau sống và gia vị đặc trưng như hành, ngò, tương ớt. Bánh mì Việt Nam nổi tiếng với hương vị đậm đà, đa dạng và là một món ăn phổ biến trong cả buổi sáng và buổi tối." },
                new Product(){ Id = Guid.NewGuid(), Name = "Bánh mực", Price = 25000, Image = "/images/products/banhmuc.jpg", Description = "Bánh mực chiên là một món ăn đặc sản của nhiều vùng ven biển Việt Nam. Bánh mực chiên được làm từ mực tươi ngon được chế biến thành một lớp bánh mỏng, giòn tan bên ngoài và mềm mịn bên trong. Mực được chiên với dầu nóng, tạo nên một màu vàng sánh và một hương vị đặc trưng. Bánh mực chiên thường được ăn kèm với nước mắm pha chua ngọt hoặc các loại sốt gia vị để tăng thêm hương vị đậm đà. Món ăn này có sự kết hợp hài hòa giữa vị giòn, thơm ngon và hương vị đặc trưng của mực, tạo nên một món ngon hấp dẫn cho thực khách." },
                new Product(){ Id = Guid.NewGuid(), Name = "Nước ngọt coca", Price = 15000, Image = "/images/products/coca.png", Description = "Nước ngọt Coca-Cola là một loại đồ uống có gas phổ biến trên toàn thế giới. Coca-Cola có hương vị đặc trưng, kết hợp giữa một vị ngọt ngào và một chút hơi chua. Được sản xuất từ các thành phần tự nhiên và hương liệu tổng hợp, Coca-Cola mang đến cho người uống cảm giác sảng khoái và hương vị thú vị. Nước ngọt Coca-Cola có thể được thưởng thức mát lạnh hoặc có thể sử dụng làm thành phần trong các món cocktail và đồ uống pha chế. Với hương vị đặc trưng và khả năng kết hợp linh hoạt, Coca-Cola đã trở thành một lựa chọn phổ biến trong việc thưởng thức đồ uống ngọt ngào và sảng khoái." },
                new Product(){ Id = Guid.NewGuid(), Name = "Khoai tây chiên + Humbeger", Price = 50000, Image = "/images/products/combokhoaitayhumbeger.jpg", Description = "Combo humburger và khoai tây chiên là một sự kết hợp tuyệt vời trong ẩm thực. Humburger với bánh mì mềm mịn, thịt bò xay thơm ngon, rau sống tươi mát và sốt phong phú, tạo nên một khẩu phần ăn ngon lành và bổ dưỡng. Khoai tây chiên với vỏ ngoài giòn tan và mềm bên trong, là món ăn vặt truyền thống và phổ biến. Khi kết hợp với humburger, khoai tây chiên tạo nên sự cân bằng giữa vị mặn, ngọt và giòn tan, làm tăng thêm hương vị và sự thú vị cho bữa ăn. Combo này thường được yêu thích trong các nhà hàng fast food và là một lựa chọn phổ biến cho bữa trưa hoặc bữa tối nhanh gọn." },
                new Product(){ Id = Guid.NewGuid(), Name = "Bánh Humbeger", Price = 40000, Image = "/images/products/humbeger.jpg", Description = "Hamburger là một món ăn nhanh phổ biến trên khắp thế giới. Nó bao gồm một ổ bánh mì mềm, thường có hình dạng tròn, được chia thành hai lớp. Giữa hai lớp bánh mì, có một miếng thịt bò chiên hoặc nướng, thường được gia vị và nướng đến khi chín vừa. Bên cạnh thịt bò, hamburger thường được kèm theo các thành phần như rau sống (như rau diếp, cà chua, hành tây), phô mai, xúc xích, gia vị và sốt. Hamburger là một món ăn ngon, tiện lợi và thường được dùng trong các nhà hàng, quán ăn nhanh và thậm chí là để tự nấu tại nhà." },
                new Product(){ Id = Guid.NewGuid(), Name = "Kem ly", Price = 25000, Image = "/images/products/kemly.jpg", Description = "Kem ly là một loại món tráng miệng ngọt thường được phục vụ trong các cửa hàng kem, quầy kem hoặc nhà hàng. Nó bao gồm một viên kem được đặt lên một chiếc ly hoặc cốc. Kem ly có nhiều hương vị và đa dạng trong cách trang trí. Nó thường được thưởng thức với các loại topping như nước sốt, hạt dẻ, chocolate, trái cây tươi, bánh quy hoặc whipped cream. Kem ly là một món ăn giải nhiệt thú vị và thường được yêu thích bởi người tiêu dùng, đặc biệt là trong những ngày hè nóng bức." },
                new Product(){ Id = Guid.NewGuid(), Name = "Khoai tây chiên", Price = 15000, Image = "/images/products/khoaitaychien.jpg", Description = "Khoai tây chiên là một món ăn phổ biến và ngon miệng. Khoai tây được cắt thành miếng dài, sau đó chiên trong dầu nóng cho đến khi chúng có màu vàng và bề mặt giòn rụm. Khoai tây chiên thường được gia vị với muối và có thể được kèm theo sốt mù tạt, mayonnaise hoặc các loại sốt khác để tạo thêm hương vị. Món khoai tây chiên là một món ăn nhanh phổ biến trong các nhà hàng, quán ăn và thậm chí cả trong các bữa tiệc. Với hương vị ngon, vị giòn và thích hợp để ăn kèm với nhiều loại món, khoai tây chiên là một món ăn mà nhiều người thích và thưởng thức." },
                new Product(){ Id = Guid.NewGuid(), Name = "Matcha kem", Price = 25000, Image = "/images/products/matchakem.jpg", Description = "Matcha kem là một loại kem được làm từ bột matcha - một loại trà xanh nguyên chất từ Nhật Bản. Kem matcha có màu xanh đặc trưng và hương vị đắng ngọt, thơm mát. Nó thường được sử dụng làm nguyên liệu chính trong các món tráng miệng như kem ly, bánh, nước ép, kem bơ, và thậm chí trong các đồ uống như latte matcha. Matcha kem là sự kết hợp hoàn hảo giữa vị đắng của trà xanh matcha và độ ngọt của kem, tạo nên một hương vị độc đáo và hấp dẫn cho người thưởng thức. Nó thường được ưa chuộng bởi những người yêu thích trà xanh và muốn thưởng thức hương vị trà xanh đặc trưng trong một món kem ngon." },
                new Product(){ Id = Guid.NewGuid(), Name = "Mì tôm", Price = 20000, Image = "/images/products/mitom.jpg", Description = "Mì tôm là một món ăn nhanh phổ biến và tiện lợi. Nó gồm mì xào chín kỹ cùng với gói gia vị có sẵn đi kèm, thường là gia vị mì tôm hỗn hợp. Mì tôm có hương vị mặn, ngọt và đậm đà, tạo nên một món ăn thú vị và dễ ăn. Nó được ưa thích bởi sự tiện lợi, nhanh chóng và đa dạng trong cách chế biến. Mì tôm có thể được thêm gia vị, rau sống, thịt hoặc hải sản để tăng thêm độ ngon và đa dạng hương vị." },
                new Product(){ Id = Guid.NewGuid(), Name = "Mì xào thập cẩm", Price = 30000, Image = "/images/products/mixao.jpg", Description = "Mỳ xào là một món ăn ngon và phổ biến trong nhiều nền văn hóa ẩm thực. Nó bao gồm mỳ được xào chín cùng với các nguyên liệu khác như thịt, hải sản, rau củ, gia vị và sốt. Mỳ xào có nhiều hương vị đa dạng, từ mỳ xào hải sản tươi ngon đến mỳ xào thịt bò thơm phức. Nó có một hương vị đậm đà và thường được nêm nếm với các loại gia vị như nước mắm, xì dầu, tỏi, hành và gia vị khác. Mỳ xào thường được chế biến nhanh chóng và phục vụ như một món ăn trưa hoặc tối tiện lợi và ngon miệng." },
                new Product(){ Id = Guid.NewGuid(), Name = "Nước chanh", Price = 15000, Image = "/images/products/nuocchanh.jpg", Description = "Nước chanh là một loại đồ uống phổ biến được làm từ nước cốt chanh tươi và đường. Nó có vị chua mát, thường được người ta ưa thích trong những ngày nóng. Nước chanh cũng có thể được kết hợp với đá, mật ong hoặc các loại thảo mộc khác để tạo thêm hương vị đặc biệt. Đồ uống này không chỉ thơm ngon mà còn cung cấp vitamin C và các chất chống oxy hóa, có thể giúp tăng cường hệ miễn dịch và cung cấp năng lượng cho cơ thể. Nước chanh thường được thưởng thức trong các buổi tiệc, họp mặt và cũng là một lựa chọn tuyệt vời để giải khát trong cuộc sống hàng ngày." },
                new Product(){ Id = Guid.NewGuid(), Name = "Nước chanh dây", Price = 20000, Image = "/images/products/nuocchanhday.jpg", Description = "Nước chanh dây là một loại nước giải khát được làm từ nước ép hoặc cốt chanh dây tươi. Chanh dây có hương vị độc đáo, chua nhẹ kết hợp với hương thơm tự nhiên. Nước chanh dây thường được pha chế bằng cách trộn nước chanh dây tươi với nước, đường và đá để tạo nên một đồ uống mát lạnh và thơm ngon. Nước chanh dây có hàm lượng vitamin C cao, làm tăng sự tươi mát và sảng khoái trong ngày nóng. Nó cũng có thể được thêm vào các món cocktail hoặc sử dụng làm thành phần chính trong các món tráng miệng và đồ uống khác. Nước chanh dây là một lựa chọn phổ biến để thưởng thức trong các quán cà phê, quán bar và các sự kiện giải trí." },
                new Product(){ Id = Guid.NewGuid(), Name = "Pepsi", Price = 15000, Image = "/images/products/pepsi.jpg", Description = "Pepsi là một thương hiệu nước ngọt nổi tiếng trên toàn thế giới. Được ra mắt vào năm 1893, Pepsi có hương vị đặc trưng và phong cách riêng. Nước ngọt Pepsi có hương vị ngọt mát, kết hợp giữa các thành phần carbonated (có ga) và hương vị cola. Đây là một loại đồ uống phổ biến và thường được thưởng thức như một lựa chọn giải khát trong các bữa ăn hoặc các dịp vui chơi, hội họp. Pepsi cũng có các phiên bản khác nhau như Pepsi Light (không calo), Pepsi Max (không đường) và Pepsi Twist (có hương chanh). Nước ngọt Pepsi có sự đối thủ cạnh tranh với Coca-Cola và là một trong những thương hiệu nước ngọt được yêu thích trên thị trường." },
                new Product(){ Id = Guid.NewGuid(), Name = "Phở bò", Price = 35000, Image = "/images/products/phobo.jpg", Description = "Phở bò là một món ăn truyền thống của Việt Nam. Nó bao gồm một tô nước dùng phở thơm ngon, có hương vị đậm đà từ xương bò, được ăn kèm với bánh phở mềm mịn và thịt bò thái mỏng. Ngoài thịt bò, phở bò còn có thể có các thành phần như gân, sách, nạm và chả bò. Bên trên tô phở, người ta thường thêm gia vị như rau mùi, hành lá, ngò gai, hành phi, hạt tiêu, và chanh để tạo thêm hương vị tươi mát và hấp dẫn. Phở bò là một món ăn rất phổ biến và được ưa chuộng trong ẩm thực Việt Nam, được thưởng thức vào bữa sáng, trưa hoặc tối." },
                new Product(){ Id = Guid.NewGuid(), Name = "Bánh Pizza", Price = 150000, Image = "/images/products/pizza.jpg", Description = "Pizza là một món ăn nổi tiếng trên toàn thế giới, xuất phát từ Ý. Nó được làm từ bột mì, được nướng chảo thành một chiếc bánh mỏng hoặc dày. Bề mặt của pizza được phết đầy các loại sốt, như sốt cà chua, sốt bơ tỏi, hoặc sốt hành. Sau đó, nó được phủ đều các loại phô mai và những nguyên liệu khác như xúc xích, thịt, rau củ, hải sản, hoặc hành tây. Cuối cùng, pizza được nướng trong lò nhiệt đới cho đến khi phô mai tan chảy và bề mặt có màu vàng nâu hấp dẫn. Pizza có nhiều hương vị và biến thể khác nhau, từ pizza Margherita truyền thống đến pizza hawaiian với cảm quan ngọt của thịt dứa. Nó là một món ăn phổ biến trong các nhà hàng, quán ăn và cũng có thể được tùy chỉnh theo khẩu vị cá nhân." },
                new Product(){ Id = Guid.NewGuid(), Name = "Thịt gà luộc", Price = 120000, Image = "/images/products/thitga.jpg", Description = "Thịt gà luộc là một món ăn đơn giản và phổ biến, được chuẩn bị bằng cách luộc thịt gà cho đến khi chín mềm và thấm đều gia vị. Thường thì gà được làm sạch, sau đó luộc trong nước sôi với các gia vị như muối, hành, gừng và một số gia vị khác tùy theo khẩu vị. Quá trình luộc thường kéo dài khoảng 30-40 phút cho đến khi thịt gà chín hoàn toàn.\r\n\r\nThịt gà luộc có thịt mềm, mịn và thấm đều hương vị từ gia vị. Món này thường được dùng như món ăn chính, có thể kèm theo nước mắm pha chua ngọt, nước sốt hoặc các loại gia vị tùy thích. Thịt gà luộc cũng thường được sử dụng để làm các món ăn khác như gỏi gà, bánh mì gà luộc, phở gà và nhiều món ăn khác.\r\n\r\nThịt gà luộc là một món ăn đơn giản nhưng thơm ngon, giàu chất dinh dưỡng và phù hợp để thưởng thức vào bất kỳ dịp nào." },
                new Product(){ Id = Guid.NewGuid(), Name = "Thịt xiên nướng", Price = 10000, Image = "/images/products/thitnuong.jpg", Description = "Thịt xiên nướng là một món ăn phổ biến trong nhiều nền văn hóa ẩm thực trên thế giới. Món này thường được chuẩn bị bằng cách xiên các miếng thịt lên que tre hoặc que kim loại, sau đó nướng trên lửa than hoặc lửa than củi. Thịt xiên nướng có thể là thịt heo, thịt gà, thịt bò, thịt cừu hoặc hải sản như tôm, cá, mực. Trước khi nướng, thịt thường được ướp gia vị hoặc sốt để tăng thêm hương vị. Thịt xiên nướng thường có mùi thơm hấp dẫn, vị ngon và có độ giòn mềm tùy theo cách nướng và loại thịt được sử dụng. Món này thường được thưởng thức kèm theo các loại nước mắm pha chua ngọt, nước sốt hoặc các loại gia vị khác. Thịt xiên nướng là một món ăn phổ biến trong các buổi tiệc ngoài trời, nhà hàng và quán ăn." },
                new Product(){ Id = Guid.NewGuid(), Name = "Trà dâu", Price = 12000, Image = "/images/products/tradau.jpg", Description = "Trà dâu là một loại thức uống phổ biến được làm từ trà đen hoặc trà xanh kết hợp với hương vị dâu tươi. Để tạo nên trà dâu, trà được pha chế và sau đó được pha trộn với nước hoa quả dâu tươi hoặc siro dâu để tạo ra một hương vị thơm ngon và màu sắc hấp dẫn.\r\n\r\nTrà dâu thường có hương vị hòa quyện giữa vị chát nhẹ và ngọt nhẹ từ trà, kết hợp với hương thơm tự nhiên và chua ngọt của dâu. Đây là một loại đồ uống mát lạnh rất thích hợp để thưởng thức trong những ngày nóng, cung cấp cảm giác sảng khoái và hương vị ngọt ngào.\r\n\r\nTrà dâu có thể được phục vụ lạnh hoặc đá, có thể được thêm đường, đá viên, hoặc tráng miệng với kem tươi. Ngoài ra, bạn cũng có thể thêm các loại trái cây tươi, lá bạc hà hoặc thêm một chút nước cốt chanh để tạo thêm độ tươi mát cho trà dâu.\r\n\r\nTrà dâu là một lựa chọn thú vị và ngon miệng để thưởng thức trong những buổi chiều hoặc khi bạn cần một ly thức uống giải khát đầy màu sắc." },
                new Product(){ Id = Guid.NewGuid(), Name = "Trà dứa", Price = 12000, Image = "/images/products/tradua.jpg", Description = "Trà dứa là một loại thức uống truyền thống của Việt Nam, được làm từ lá dứa tươi và trà đen. Đây là sự kết hợp hoàn hảo giữa hương vị thơm ngon của lá dứa và hương trà đậm đà. Trà dứa thường được pha lạnh và có thể được thêm đá hoặc tráng miệng với đường phèn. Đây là một thức uống mát lạnh, thích hợp để giải khát và thưởng thức trong những ngày nóng." },
                new Product(){ Id = Guid.NewGuid(), Name = "Viên xiên thập cẩm", Price = 20000, Image = "/images/products/xienban.jpg", Description = "Viên chiên lề đường là một món ăn đường phố phổ biến, thường được làm từ bột mì và đường. Chúng được chiên giòn và có hình dạng tròn nhỏ, tạo nên một lớp vỏ ngoài màu vàng hấp dẫn. Khi cắn vào, viên chiên lề đường mang đến hương vị ngọt ngào từ đường caramel hòa quyện với vị bùi bên trong. Đây là món ăn nhẹ nhàng, thường được thưởng thức như một món tráng miệng hoặc món ăn vặt ngon lành trong thời gian rảnh rỗi." }
            };
        }
    }
}
