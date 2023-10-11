using System.Threading.Tasks;

namespace Website.Services.Interfaces
{
    public interface IBaseReCurrentTaskService
    {
        Task RunAsync(string jobId, string url);
    }
}
