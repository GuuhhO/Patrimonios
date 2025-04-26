using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Patrimonios.Data;
using Patrimonios.Settings;
using System.Configuration;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Patrimonios.Models;
using Patrimonios.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PatrimoniosContextConnection") ?? throw new InvalidOperationException("Connection string 'PatrimoniosContextConnection' not found.");

builder.Services.AddDbContext<PatrimoniosContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PatrimoniosUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<PatrimoniosContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("pt-BR"), new CultureInfo("en-US") };
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Logging.AddConsole();

builder.Logging.AddDebug();

builder.Services.AddScoped<LogService>();

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

var supportedCultures = new[] { new CultureInfo("pt-BR"), new CultureInfo("en-US") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();

    endpoints.MapAreaControllerRoute(
        name: "Identity",
        areaName: "Identity",
        pattern: "Account/{action}/{id?}",
        defaults: new { controller = "Account", action = "Manage" });
});

app.Run();
