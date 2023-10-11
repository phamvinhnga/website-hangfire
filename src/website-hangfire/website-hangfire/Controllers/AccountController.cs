using Hangfire;
using Hangfire.Storage;
using Website.Models;
using Website.Services.Interfaces;
using Website.Views;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBaseReCurrentTaskService _baseReCurrentTaskService;
        private readonly IOptionsMonitor<List<TaskSettingOption>> _taskSettingOptions;
        private readonly IOptionsMonitor<SystemUserOption> _systemUserOption;

        public AccountController(
            IBaseReCurrentTaskService baseReCurrentTaskService,
            IOptionsMonitor<SystemUserOption> systemUserOption,
            IAccountService accountService,
            IOptionsMonitor<List<TaskSettingOption>> taskSettingOptions
        )
        {
            _systemUserOption = systemUserOption;
            _baseReCurrentTaskService = baseReCurrentTaskService;
            _taskSettingOptions = taskSettingOptions;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("~/hangfire");
            }
            return View(viewName: "~/Views/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: "~/Views/Login.cshtml");
            }

            var isSuccess = AuthenticateUser(input.Username, input.Password);
            if (!isSuccess)
            {
                return View(viewName: "~/Views/Login.cshtml");
            }

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>()
                {
                     new Claim(ClaimTypes.Name, _systemUserOption.CurrentValue.Username)
                }, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties());

            BuildRecurringJob();
            return Redirect("~/hangfire");
        }

        private void BuildRecurringJob()
        {
            using(var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            foreach (var task in _taskSettingOptions.CurrentValue)
            {
                RecurringJob.AddOrUpdate(
                    recurringJobId: task.TaskName,
                    methodCall: () => _baseReCurrentTaskService.RunAsync(task.TaskName, task.Url),
                    cronExpression: Cron.Daily(minute: task.Minute, hour: task.Hour));
            }
        }

        private bool AuthenticateUser(string Username, string password)
        {
            string UsernameLogin = _systemUserOption.CurrentValue.Username;
            string passwordLogin = _systemUserOption.CurrentValue.Password;
            return Username == UsernameLogin && password == passwordLogin;
        }
    }
}