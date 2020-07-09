using Microsoft.Extensions.DependencyInjection;

namespace MyLab.Indexer
{
    public interface ISingletonRegistrar<TService>
    {
        IServiceCollection Register(IServiceCollection serviceCollection);
    }

    public class GenericSingletonRegistrar<TService, TImplementation> : ISingletonRegistrar<TService>
        where TImplementation : class, TService
        where TService : class
    {
        public IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<TService, TImplementation>();
        }
    }

    public class ObjectSingletonRegistrar<TService> : ISingletonRegistrar<TService>
        where TService : class
    {
        private readonly TService _implementation;

        public ObjectSingletonRegistrar(TService implementation)
        {
            _implementation = implementation;
        }

        public IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<TService>(_implementation);
        }
    }
}