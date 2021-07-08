using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using FunctionApp;
using Intercom;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(Startup))]

namespace FunctionApp
{
    public class Startup : FunctionsStartup
    {

        private string IntercomApiKey => Environment.GetEnvironmentVariable("IntercomApiKey");

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IIntercomService, IntercomService>(provider => 
                new IntercomService(provider.GetService<IHttpClientFactory>(), IntercomApiKey)
            );
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }
    }
}
