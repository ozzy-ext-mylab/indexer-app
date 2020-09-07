using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Indexer;
using MyLab.Indexer.Services;
using MyLab.Indexer.Tools;
using MyLab.Mq;

namespace IntegrationTests
{
    public class IndexerLogicBehavior
    {
        private static string EsAlias = "foo-index";

        IInputMessageEmulator InitTestLogic(IOriginEntitySource originEntitySource, IIndexManager indexManager)
        {
            var memConfig = new Dictionary<string,string>
            {
                { "InputQueue", "test-queue" }
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(memConfig);

            var config = configBuilder.Build();

            var services = new ServiceCollection();

            var logic = new IndexerConsumerLogic(
                originEntitySource,
                indexManager,
                new IndexAppOptions
                {
                    EsAlias = EsAlias,
                    PageSize = 10
                },
                null);

            services.AddIndexerLogic(config, logic, new InputMessageEmulatorRegistrar());

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetService<IInputMessageEmulator>();
        }
    }
}
