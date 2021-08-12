using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScopedRegistrationsRepro
{
    
    public interface IMessageProvider
    {
        Task<string> Provide<TMessage>();
    }

    public interface IMessageProvider<TMessage>
    {
        Task<string> Provide();
    }

    public interface IMessage
    {

    }

    public class HelloMessage : IMessage
    {
    }

    public class HeaderMessage : IMessage {}

    public class HelloMessageProvider : IMessageProvider<HelloMessage>
    {
        private readonly IUserIdentityAccessor accessor;

        public HelloMessageProvider(IUserIdentityAccessor accessor)
        {
            this.accessor = accessor;
        }

        public async Task<string> Provide()
        {
            var current = await accessor.GetCurrentIdentity();
            return $"Hello - {current}";
        }
    }

    public class HeaderMessageProvider : IMessageProvider<HeaderMessage>
    {
        private readonly IUserIdentityAccessor accessor;

        public HeaderMessageProvider(IUserIdentityAccessor accessor)
        {
            this.accessor = accessor;
        }

        public async Task<string> Provide()
        {
            return await accessor.GetCurrentIdentity();
        }
    }


    public class MessageProvider : IMessageProvider
    {
        private readonly IServiceProvider serviceProvider;
        public MessageProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        public async Task<string> Provide<TMessage>()
        {
            var provider = serviceProvider.GetRequiredService<IMessageProvider<TMessage>>();
            if (provider == null)
            {
                throw new InvalidOperationException();
            }

            return await provider.Provide();
        }
    }
}

