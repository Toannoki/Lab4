using ASC.Web.Data;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ASC.Web.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using ASC.DataAccess.Interfaces;
using ASC.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
	throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ✅ Đăng ký Identity chỉ một lần và cấu hình bên trong luôn
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<DbContext, ApplicationDbContext>();

builder.Services.AddOptions();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();
builder.Services.AddSingleton<IIdentitySeed, IdentitySeed>();
builder.Services.AddScoped<IUniOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();

// ✅ Đảm bảo có app.UseAuthentication()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
	var storageSeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
	await storageSeed.Seed(scope.ServiceProvider.GetService<UserManager<IdentityUser>>(),
		scope.ServiceProvider.GetService<RoleManager<IdentityRole>>(),
		scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>());
}

app.Run();
