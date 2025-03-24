using ASC.DataAccess;
using ASC.DataAccess.Interfaces;
using ASC.Web.Configuration;
using ASC.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddCongfig(this IServiceCollection services, IConfiguration config)
		{
			var connectionString = config.GetConnectionString("DefaultConnection") ??
				throw new InvalidOperationException("Connection string not found");
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			services.AddOptions();
			services.Configure<ApplicationSettings>(config.GetSection("AppSettings"));
			return services;
		}

		public static IServiceCollection AddMyDependencyGroup(this IServiceCollection services)
		{
			services.AddScoped<DbContext, ApplicationDbContext>();

			services.AddIdentity<IdentityUser, IdentityRole>(options =>
			{
				options.User.RequireUniqueEmail = true;
			}).AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			// Add services
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddSingleton<IIdentitySeed, IdentitySeed>();
			services.AddScoped<IUniOfWork, UnitOfWork>();

			// Add Cache, Session
			services.AddSession();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddDistributedMemoryCache();
			services.AddSingleton<INavigationCacheOperations, NavigationCacheOperations>();

			services.AddRazorPages();
			services.AddDatabaseDeveloperPageExceptionFilter();
			services.AddControllersWithViews();

			return services;
		}
	}
}
