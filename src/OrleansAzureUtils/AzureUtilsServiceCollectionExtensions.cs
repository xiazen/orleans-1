﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.AzureUtils.Configuration;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.MembershipService;

namespace Microsoft.Orleans.Hosting
{
    public static class AzureUtilsServiceCollectionExtensions
    {
        /// <summary>
        /// Configure DI container to use AzureTableBasedMemebership
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        public static IServiceCollection UseAzureTableMemebership(this IServiceCollection services,
            Action<AzureTableMembershipOptions> configureOptions)
        {
            services.Configure<AzureTableMembershipOptions>(configureOptions);
            services.AddSingleton<IMembershipTable, AzureBasedMembershipTable>();
            return services;
        }

        /// <summary>
        /// Configure DI container to use AzureTableBasedMemebership
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection UseAzureTableMemebership(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AzureTableMembershipOptions>(config);
            services.AddSingleton<IMembershipTable, AzureBasedMembershipTable>();
            return services;
        }
    }
}
