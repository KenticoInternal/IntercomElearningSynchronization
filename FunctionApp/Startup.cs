using System;
using System.Net.Http;
using Business;
using ElearningData;
using FunctionApp;
using Intercom;
using Kontent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
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

            builder.Services.AddScoped<IKontentService, KontentService>();
            builder.Services.AddScoped<IBusinessService, BusinessService>();
            builder.Services.AddScoped<IElearningDataService, ElearningDataService>();
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
