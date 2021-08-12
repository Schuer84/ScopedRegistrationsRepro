using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScopedRegistrationsRepro
{
    public interface IMessageService
    {
        Task<string> Hello();
        Task<string> Header();
    }

    public interface IValidationService
    {
        Task<string> Validate();
    }

    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider provider;

        public ValidationService(IServiceProvider provider)
        {
            this.provider = provider;
        }


        public async Task<string> Validate()
        {
            var identityAccessor = provider.GetService<IUserIdentityAccessor>();
            return await identityAccessor.GetCurrentIdentity();
        }
    }


    public  class MessageService : IMessageService
    {
        private readonly IMessageProvider messageProvider;
        public MessageService(IMessageProvider messageProvider)
        {
            this.messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        }

        public async Task<string> Hello()
        {
            return await messageProvider.Provide<HelloMessage>();
        }

        public async Task<string> Header()
        {
            return await messageProvider.Provide<HeaderMessage>();
        }
    }
}
