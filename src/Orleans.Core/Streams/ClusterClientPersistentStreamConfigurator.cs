﻿using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Streams
{
    public interface IClusterClientPersistentStreamConfigurator
    {
        IClusterClientPersistentStreamConfigurator Configure<TOptions>(Action<OptionsBuilder<TOptions>> configureOptions)
        where TOptions : class, new();
    }

    public static class ClusterClientPersistentStreamConfiguratorExtensions
    {
        public static IClusterClientPersistentStreamConfigurator ConfigureInitialization(this IClusterClientPersistentStreamConfigurator configurator, Action<OptionsBuilder<StreamInitializationOptions>> configureOptions)
        {
            configurator.Configure<StreamInitializationOptions>(configureOptions);
            return configurator;
        }

        public static IClusterClientPersistentStreamConfigurator ConfigureStreamPubSub(this IClusterClientPersistentStreamConfigurator configurator, StreamPubSubType pubsubType = StreamPubSubOptions.DEFAULT_STREAM_PUBSUB_TYPE)
        {
            configurator.Configure<StreamPubSubOptions>(ob => ob.Configure(options => options.PubSubType = pubsubType));
            return configurator;
        }
    }

    public class ClusterClientPersistentStreamConfigurator : IClusterClientPersistentStreamConfigurator
    {
        protected readonly string name;
        protected readonly IClientBuilder clientBuilder;
        public ClusterClientPersistentStreamConfigurator(string name, IClientBuilder clientBuilder)
        {
            this.name = name;
            this.clientBuilder = clientBuilder;
            //wire stream provider into lifecycle 
            this.clientBuilder.ConfigureServices(services => this.AddPersistentStream(services));
        }

        private void AddPersistentStream(IServiceCollection services)
        {
            //wire the stream provider into life cycle
            services.AddSingletonNamedService<IStreamProvider>(name, PersistentStreamProvider.Create)
                           .AddSingletonNamedService<ILifecycleParticipant<IClusterClientLifecycle>>(name, (s, n) => ((PersistentStreamProvider)s.GetRequiredServiceByName<IStreamProvider>(n)).ParticipateIn<IClusterClientLifecycle>())
                           .AddSingletonNamedService(name, (s, n) => s.GetServiceByName<IStreamProvider>(n) as IControllable)
                           .ConfigureNamedOptionForLogging<StreamInitializationOptions>(name);
        }

        public IClusterClientPersistentStreamConfigurator Configure<TOptions>(Action<OptionsBuilder<TOptions>> configureOptions) where TOptions : class, new()
        {
            clientBuilder.ConfigureServices(services =>
            {
                configureOptions?.Invoke(services.AddOptions<TOptions>(this.name));
            });
            return this;
        }
    }
}
