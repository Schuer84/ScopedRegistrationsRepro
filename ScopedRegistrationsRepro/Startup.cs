using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ScopedRegistrationsRepro;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ScopedRegistrationsRepro
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = CreateConfiguration(builder.GetContext());

            builder.Services.AddWebJobs(x => { });
            // Utilities
            builder.Services.AddHttpClient();
            builder.Services.AddLogging();
            builder.Services
                .AddMvcCore()
                .AddNewtonsoftJson(x => ConfigureSerializerSettings(x.SerializerSettings));

            builder.Services.AddScoped<IUserIdentityAccessor, UserIdentityAccessor>();
            builder.Services.AddSingleton<IMessageProvider, MessageProvider>();
            builder.Services.AddTransient<IMessageProvider<HelloMessage>, HelloMessageProvider>();
            builder.Services.AddTransient<IMessageProvider<HeaderMessage>, HeaderMessageProvider>();

            builder.Services.AddTransient<IValidationService, ValidationService>();
            builder.Services.AddTransient<IMessageService, ScopedRegistrationsRepro.MessageService>();

        }

        private static IConfigurationRoot CreateConfiguration(FunctionsHostBuilderContext context)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureSerializerSettings(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters = new List<JsonConverter>()
            {
                new StringEnumConverter(new CamelCaseNamingStrategy()),
            };

            JsonConvert.DefaultSettings = () => settings;
        }
    }
}
