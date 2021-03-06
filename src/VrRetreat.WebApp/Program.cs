using BenjaminAbt.HCaptcha.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VrRetreat.Core;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Infrastructure;
using VrRetreat.Infrastructure.Entities;
using VrRetreat.WebApp.Extensions;
using VrRetreat.WebApp.Factory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<VrRetreatUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
#if DEBUG
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
#endif
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<VrRetreatUser>, CustomClaimsFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBioCodeGenerator, BioCodeGenerator>();
builder.Services.AddSingleton<IVrChat, VrChat>();
builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<VrRetreat.Core.IConfiguration>(sp => JsonConfiguration.FromFile("VrChatConfig.json"));
builder.Services.AddUseCases();
builder.Services.AddPresenters();

builder.Services.AddHCaptcha(builder.Configuration.GetSection("HCaptcha"));

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(mvcOptions => mvcOptions.AddHCaptchaModelBinder());

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
