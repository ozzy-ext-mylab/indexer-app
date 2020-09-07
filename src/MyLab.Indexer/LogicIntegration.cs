using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Indexer.Services;
using MyLab.Mq;

namespace MyLab.Indexer
{
    static class LogicIntegration
    {
        public static IServiceCollection AddIndexerLogic(
            this IServiceCollection services, 
            IConfiguration config,
            IndexerConsumerLogic logic,
            IInitiatorRegistrar initiatorRegistrar)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var targetQueue = GetQueueFromConfig(config);

            var consumer = new MqConsumer<IndexingMsg, IndexerConsumerLogic>(targetQueue, logic);

            return services.AddMqConsuming(
                consumerRegistrar => 
                    consumerRegistrar.RegisterConsumer(consumer), initiatorRegistrar);
        }

        public static IServiceCollection AddIndexerLogic(
            this IServiceCollection services,
            IConfiguration config)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var targetQueue = GetQueueFromConfig(config);

            var consumer = new MqConsumer<IndexingMsg, IndexerConsumerLogic>(targetQueue);

            return services.AddMqConsuming(
                consumerRegistrar =>
                    consumerRegistrar.RegisterConsumer(consumer));
        }

        static string GetQueueFromConfig(IConfiguration config)
        {
            var targetQueue = config["InputQueue"];

            if (string.IsNullOrWhiteSpace(targetQueue))
                throw new ConfigurationErrorsException("There is no configuration parameter InputQueue");

            return targetQueue;
        }

    }
}
