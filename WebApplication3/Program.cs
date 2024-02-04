using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WebApplication3;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
/*builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptcha"));*/
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;

    // Account lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Lockout duration
    options.Lockout.MaxFailedAccessAttempts = 3; // Number of allowed failed login attempts
    options.Lockout.AllowedForNewUsers = true; // Allow lockout for new users
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/Account/AccessDenied";

});
builder.Services.AddScoped<ITokenGenerator, RandomTokenGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

/*app.UseStatusCodePagesWithRedirects("/errors/{0}");*/
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession(); 
app.MapRazorPages();

app.Run();
