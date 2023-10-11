using Hangfire;
using Hangfire.Dashboard;
using Website.Filters;
using Website.Models;
using Website.Services;
using Website.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<List<TaskSettingOption>>()
    .Bind(builder.Configuration
    .GetSection(TaskSettingOption.Position));
builder.Services.AddOptions<SystemUserOption>()
    .Bind(builder.Configuration
    .GetSection(SystemUserOption.Position))
    .ValidateDataAnnotations();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddHealthChecks();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/";
});

builder.Services.AddTransient<IBaseReCurrentTaskService, BaseReCurrentTaskService>();
builder.Services.AddTransient<IAccountService, AccountService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
var dashboardOptions = new DashboardOptions()
{
    Authorization = new IDashboardAuthorizationFilter[] { new HangfireAuthorizationFilter() }
};
app.MapHangfireDashboard("/hangfire", dashboardOptions).RequireAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

app.Run();
