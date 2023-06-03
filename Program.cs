using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcWedBanDungCuHocTap.Models;
using MvcWedBanDungCuHocTap.Repositories.Abstract;
using MvcWedBanDungCuHocTap.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI
builder.Services.AddDbContext<DbApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDatabase"))
);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DbApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/UserAuthentication/Login");
//add services to container
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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



app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Admin",
    pattern: "Admin/{controller=DashBoard}/{action=Index}/{Id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default",
        "{controller}/{action}/{id?}",
        new { controller = "Home", action = "Index" },
        new { controller = @"^(?!User).*$" }// exclude user controller
    );

app.Run();
