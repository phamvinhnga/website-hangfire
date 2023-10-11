using Website.Services.Interfaces;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace Website.Services
{
    public class AccountService : IAccountService
    {
        private readonly IOptionsMonitor<SystemUserOption> _systemUserOption;

        public AccountService(
            IOptionsMonitor<SystemUserOption> systemUserOption) 
        {
            _systemUserOption = systemUserOption;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new
                {
                    username = username,
                    password = password,
                    remember = false
                };

                var response = await client.PostAsJsonAsync(_systemUserOption.CurrentValue.Url, payload);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Cannot login system - StatusCode: {response.StatusCode}");
                }
                var strRes = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccountLogin>(strRes).AccessToken;
            }
        }
    }
}
