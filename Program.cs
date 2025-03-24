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

builder.Services.AddCongfig(builder.Configuration).AddMyDependencyGroup();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
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

app.UseAuthorization();

app.MapControllerRoute(
	name: "areaRoute",
	pattern: "{area:exists}/{controller=Home}/{action=Index}");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.UseSession();

// Seed user data
using (var scope = app.Services.CreateScope())
{
	var storageSeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
	await storageSeed.Seed(scope.ServiceProvider.GetService<UserManager<IdentityUser>>(),
		scope.ServiceProvider.GetService<RoleManager<IdentityRole>>(),
		scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>());
}

using (var scope = app.Services.CreateScope())
{
	var navigationCacheOperations = scope.ServiceProvider.GetRequiredService<INavigationCacheOperations>();
	await navigationCacheOperations.CreateNavigationCacheAsync();
}

app.Run();
