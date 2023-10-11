using System.Threading.Tasks;

namespace Website.Services.Interfaces
{
    public interface IAccountService
    {
        Task<string> LoginAsync(string username, string password);
    }
}
