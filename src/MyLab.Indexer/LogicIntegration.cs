using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Db;
using MyLab.Elastic;
using MyLab.Indexer.Services;
using MyLab.Indexer.Tools;
using MyLab.Mq;

namespace MyLab.Indexer
{
    static class LogicIntegration
    {
        public static IServiceCollection AddIndexerLogic(
            this IServiceCollection services,
            IConfiguration config,
            IInitiatorRegistrar initiatorRegistrar = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var addConfig = GetAddConfig(config);

            var consumer = new MqConsumer<IndexingMsg, IndexerConsumerLogic>(addConfig.Queue);
            var dataProvider = DataProviders.Get(addConfig.DbProvider);

            return services
                .Configure<IndexAppDbOptions>(config.GetSection("DB"))
                .Configure<IndexAppEsOptions>(config.GetSection("ElasticSearch"))
                .Configure<IndexAppLogicOptions>(config.GetSection("Indexer"))
                .Configure<IndexAppMqOptions>(config.GetSection("Mq"))
                .AddSingleton<IOriginEntitySource, DbOriginEntitySource>()
                .AddMqConsuming(consumerRegistrar => consumerRegistrar.RegisterConsumer(consumer), initiatorRegistrar)
                .AddEsTools(config)
                .AddDbTools(config, dataProvider);
        }

        static (string Queue, string DbProvider) GetAddConfig(IConfiguration config)
        {
            return (
                config.GetValue<string>("Mq:Queue"),
                config.GetValue<string>("DB:Provider")
                );
        }

    }
}
