using System;
using LinqToDB.DataProvider;
using LinqToDB.SqlQuery;
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
        public static IServiceCollection AddIndexerLogic(this IServiceCollection srv, IConfiguration config)
        {
            return AddIndexerLogic(srv, new IntegrationConfiguration
            {
                IntegrateEsTools = true,
                Configuration = config,
                EntityIndexerRegistrar = new GenericSingletonRegistrar<IEntityIndexManager, DefaultEntityIndexManager>(),
                EntityStorageRegistrar = new GenericSingletonRegistrar<IOriginEntityStorage, DefaultOriginEntityStorage>(),
                ReportersRegistrar = new GenericSingletonRegistrar<IReporter,DefaultReporter>()
            });
        }

        internal static IServiceCollection AddIndexerLogic(this IServiceCollection srv, IntegrationConfiguration icfg)
        {
            if (icfg == null) throw new ArgumentNullException(nameof(icfg));

            if(icfg.Configuration == null) throw new InvalidOperationException("Configuration is null");
            if(icfg.EntityIndexerRegistrar == null) throw new InvalidOperationException("EntityIndexerRegistrar is null");

            var options = GetOptions(icfg.Configuration);

            RegisterOptions(srv, options);

            var dataProvider = GetDataProvider(options.Db.DbProviderName);
            var indexMsgConsumer = new MqConsumer<IndexingMsg, IndexerConsumerLogic>(options.Queue);

            icfg.EntityIndexerRegistrar.Register(srv);
            icfg.EntityStorageRegistrar.Register(srv);

            srv.AddDbTools(icfg.Configuration, dataProvider);
            srv.AddMqConsuming(registrar => registrar.RegisterConsumer(indexMsgConsumer), icfg.OverrideMqInitiatorRegistrar);
            srv.AddSingleton<OriginEntityProvider>();

            if (icfg.IntegrateEsTools)
                srv.AddEsTools(icfg.Configuration);

            return srv;
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

            configuration.GetSection(IndexerAppConst.RootConfigName).Bind(options);

            return options;
        }
    }

    class IntegrationConfiguration
    {
        public ISingletonRegistrar<IEntityIndexManager> EntityIndexerRegistrar { get; set; }

        public ISingletonRegistrar<IOriginEntityStorage> EntityStorageRegistrar { get; set; }
        public ISingletonRegistrar<IReporter> ReportersRegistrar { get; set; }

        public bool IntegrateEsTools { get; set; }

        public IInitiatorRegistrar OverrideMqInitiatorRegistrar { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}
