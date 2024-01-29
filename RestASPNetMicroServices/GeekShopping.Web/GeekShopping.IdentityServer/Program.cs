using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using GeekShopping.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// MySQl

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

builder.Services
	.AddDbContext<MySQLContext>(opts =>
	opts.UseMySql(connectionString,
	ServerVersion.AutoDetect(connectionString
	)));

// Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<MySQLContext>()
	.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
	{
		options.Events.RaiseErrorEvents = true;
		options.Events.RaiseInformationEvents = true;
		options.Events.RaiseFailureEvents = true;
		options.Events.RaiseSuccessEvents = true;
		options.EmitStaticAudienceClaim = true;
	})
		.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
			.AddInMemoryClients(IdentityConfiguration.Clients)
			.AddInMemoryApiScopes(IdentityConfiguration.Scopes)
			.AddAspNetIdentity<ApplicationUser>()
			.AddDeveloperSigningCredential();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IProfileService, ProfileService>();

var app = builder.Build();

var initializer = app.Services.CreateScope().ServiceProvider.GetService<IDbInitializer>();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

initializer.Initialize();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
