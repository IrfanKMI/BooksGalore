using BooksGalore.Db;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Dbcontext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Connect")/*,b=>b.MigrationsAssembly("BooksGalore.Models")*/));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(/*options => options.SignIn.RequireConfirmedAccount = true*/).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<Dbcontext>();
builder.Services.AddSingleton<IEmailSender, EmailSender>(); //dummy email sender in first place as we override Default identity inorder to include roles, default email sender will not be available
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.Configure<BooksGalore.Utility.Stripe>(builder.Configuration.GetSection("Stripe"));
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";

});

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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();//for this we have to use get not tostring
app.UseAuthentication();


app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();