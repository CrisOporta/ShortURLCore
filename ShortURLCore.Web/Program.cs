using Microsoft.EntityFrameworkCore;
using ShortURLCore.Infrastructure.Data;
using ShortURLCore.Infrastructure.Repositories;
using ShortURLCore.Infrastructure.Repositories.IRepositories;
using ShortURLCore.Web.Services;
using ShortURLCore.Web.Services.IServices;
using ShortURLCore.Web.Filters;
using ShortURLCore.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ClientTimeZoneDateFilter>();
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISetupService, SetupService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClientTimeZoneAccessor, ClientTimeZoneAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Determine client timezone from header/cookie for each request
app.UseMiddleware<ClientTimeZoneMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();


if (!File.Exists("install.lock"))
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{Area=Short}/{controller=Setup}/{action=Index}")
        .WithStaticAssets();
}
else
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{Area=Short}/{controller=UrlMappings}/{action=Index}")
        .WithStaticAssets();
}

app.MapRazorPages();


app.Run();
