using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Indexer;
using MyLab.Indexer.Tools;
using MyLab.Mq;
using Xunit;

namespace IntegrationTests
{
    public class IndexerLogicBehavior : IClassFixture<TmpDbFixture>
    {
        IInputMessageEmulator InitTestLogic(IIndexManager indexManager)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            var services = new ServiceCollection();
            
            services.AddIndexerLogic(config, new InputMessageEmulatorRegistrar());
            
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetService<IInputMessageEmulator>();
        }
    }
}
