using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Helpers;
using ShoppingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(15);
});
var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
var connectionString = configuration.GetConnectionString("db");
builder.Services.AddDbContext<ShoppingContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.LoginPath = "/Account/Login";
                    opt.AccessDeniedPath = "/Account/AccessDenied";
                    opt.ReturnUrlParameter = "redirectUrl";
                });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ShoppingContext>();
context.Database.EnsureCreated();
SeedData.SeedDatabase(context);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// custom middleware to redirect error page when exception happens
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();
