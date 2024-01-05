using WebApplication3.Data;
using Microsoft.Build.Execution;
using Stripe;
using WebApplication3.Data.Cart;
using System.Text;
using WebApplication3.Data.Services;
using WebApplication3.Data.Stripe;
using static WebApplication3.Models.StripeModel;
using WebApplication3;
using WebApplication3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Helpers;
using WebApplication3.Data.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<WebApplication3.Data.Stripe.StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));
StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("appsettings.json")
//    .Build();

//builder.Services.AddSingleton<IConfiguration>(configuration);
//builder.Services.AddHttpClient<AcronisTokenService>(client =>
//{
//    var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
//    client.BaseAddress = new Uri(configuration["AcronisApi:BaseUrl"]);
//    client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{configuration["AcronisApi:ClientId"]}:{configuration["AcronisApi:ClientSecret"]}"))}");
//});
builder.Services.AddScoped<AcronisTokenService>();
builder.Services.AddScoped<TwilioService>();
builder.Services.AddScoped<ShoppingCart>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
//builder.Services.AddSingleton(stripeSettings);
builder.Services.AddScoped<SubscriptionsService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddSession();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 7;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = true;

}
);
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();
AppDbIntializer.SeedUsersAndRolesAsync(app).Wait();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
