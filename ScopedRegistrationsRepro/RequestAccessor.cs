using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ScopedRegistrationsRepro
{
    public interface IUserIdentityAccessor
    {
        Task<string> GetCurrentIdentity(string policy = null);
    }



    public class UserIdentityAccessor : IUserIdentityAccessor
    {
        public const string Header = "x-custom-test-header";
        private readonly IHttpContextAccessor httpContextAccessor;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Guid id = Guid.NewGuid();
        public UserIdentityAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private async Task<string> LoadUserIdentity(string policy)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                if (Current != null) return Current;
                
                var request = httpContextAccessor.HttpContext.Request;

                var header = request.Headers.GetCommaSeparatedValues(Header);
                var current = $"{string.Join(",", header)}-{id}";
                Current = current;
                return current;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private string Current { get; set; }

        public async Task<string> GetCurrentIdentity(string policy = null)
        {
            return await LoadUserIdentity(policy);
        }
    }
}
