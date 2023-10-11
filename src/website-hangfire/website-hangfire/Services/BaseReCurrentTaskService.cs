using Hangfire;
using Website.Models;
using Website.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace Website.Services
{
    public class BaseReCurrentTaskService : IBaseReCurrentTaskService
    {
        private readonly IAccountService _accountService;
        private readonly IOptionsMonitor<SystemUserOption> _systemUserOption;

        public BaseReCurrentTaskService(
            IAccountService accountService,
            IOptionsMonitor<SystemUserOption> systemUserOption
        )
        {
            _systemUserOption = systemUserOption;
            _accountService = accountService;
        }

        [DisplayName("{0}ReCurrentTaskService")]
        public async Task RunAsync(string jobId, string url)
        {
            var token = await _accountService.LoginAsync(_systemUserOption.CurrentValue.Username, _systemUserOption.CurrentValue.Password);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Cannot call api job - StatusCode: {response.StatusCode}");
                }
            }
        }
    }
}
