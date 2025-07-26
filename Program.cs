using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using SupportBookingAPP.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Error/403";
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// EmailService registered as IEmailSender
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#if DEBUG
// In Development: Use dummy email sender
builder.Services.AddTransient<IEmailSender, DevEmailService>();
#else
// PROD: Use actual email service for real SMTP delivery
builder.Services.AddTransient<IEmailSender, EmailService>();
#endif

builder.Services.AddScoped<BookingService>();

builder.Services.AddTransient<EmailService>();

// Notifications & admin seeding
builder.Services.AddTransient<NotificationService>();
builder.Services.Configure<AdminUserSettings>(builder.Configuration.GetSection("AdminUser"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();
}

// Seeding + background work
await DataSeeder.SeedInitialDataAsync(app);
await app.Services.GetRequiredService<NotificationService>().RunPendingAsync();

// Routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
