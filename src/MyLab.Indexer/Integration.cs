using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Db;
using MyLab.Elastic;
using MyLab.Indexer.Services;
using MyLab.Mq;

namespace MyLab.Indexer
{
    static class Integration
    {
        public static IServiceCollection AddIndexerLogic(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var options = GetOptions(configuration);

            RegisterOptions(serviceCollection, options);

            var dataProvider = GetDataProvider(options.Db.DbProviderName);
            var indexMsgConsumer = new MqConsumer<IndexingMsg, IndexerConsumerLogic>(options.Queue);

            return serviceCollection
                .AddDbTools(configuration, dataProvider)
                .AddMqConsuming(registrar => registrar.RegisterConsumer(indexMsgConsumer))
                .AddEsTools(configuration)
                .AddSingleton<OriginEntityProvider>()
                .AddSingleton<IOriginEntityStorage, DefaultOriginEntityStorage>();
        }

        private static IDataProvider GetDataProvider(string dbDbProviderName)
        {
            return SupportedDataProviders.Instance.TryGetValue(dbDbProviderName, out var provider)
                ? provider
                : throw new NotSupportedException($"Data base provider '{dbDbProviderName}' not supported");
        }

        private static void RegisterOptions(IServiceCollection services, IndexerOptions options)
        {
            services.Configure<IndexerOptions>(o =>
            {
                o.Index = options.Index;
                o.BatchSize = options.BatchSize;
                o.Db = options.Db;
            });
        }

        private static IndexerOptions GetOptions(IConfiguration configuration)
        {
            var options = new IndexerOptions();

            configuration.GetSection("indexer").Bind(options);

            return options;
        }
    }
}
