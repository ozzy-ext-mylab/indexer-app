using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Indexer;
using MyLab.Indexer.Services;
using MyLab.Mq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class FrameworkBehavior
    {
        private readonly ITestOutputHelper _output;

        public FrameworkBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShouldSyncSelectedData()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            var config = CreateConfig();

            var idsForUpdate = new[] {"1", "2", "3"};
             
            var source = new TestEntitySource();
            var indexer = new TestEntityIndexer();
            var icfg = new IntegrationConfiguration
            {
                Configuration = config,
                EntityIndexerRegistrar = new ObjectSingletonRegistrar<IEntityIndexer>(indexer),
                EntityStorageRegistrar = new ObjectSingletonRegistrar<IOriginEntityStorage>(source),
                OverrideMqInitiatorRegistrar = new InputMessageEmulatorRegistrar()
            };

            serviceCollection.AddIndexerLogic(icfg);

            var services = serviceCollection.BuildServiceProvider();

            var msgEmulator = services.GetService<IInputMessageEmulator>();

            //Act
            var resp = await msgEmulator.Queue(new IndexingMsg
            {
                Update = idsForUpdate
            }, "bar");


            LogQueueRes(resp);

            //Assert
            Assert.True(resp.Acked);
            Assert.Equal(3, indexer.Indexed.Count);

            foreach (var id in idsForUpdate)
            {
                Assert.Contains(indexer.Indexed, e => e.Id == id);
            }
        }

        [Fact]
        public async Task ShouldSyncAllData()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            var config = CreateConfig();
            
            var source = new TestEntitySource();
            var indexer = new TestEntityIndexer(new []{ new DbEntity{Id = "100"}, new DbEntity(){ Id = "200"}, });
            var icfg = new IntegrationConfiguration
            {
                Configuration = config,
                EntityIndexerRegistrar = new ObjectSingletonRegistrar<IEntityIndexer>(indexer),
                EntityStorageRegistrar = new ObjectSingletonRegistrar<IOriginEntityStorage>(source),
                OverrideMqInitiatorRegistrar = new InputMessageEmulatorRegistrar()
            };

            serviceCollection.AddIndexerLogic(icfg);

            var services = serviceCollection.BuildServiceProvider();

            var msgEmulator = services.GetService<IInputMessageEmulator>();

            //Act
            var resp = await msgEmulator.Queue(new IndexingMsg
            {
                Reindex = true
            }, "bar");

            LogQueueRes(resp);

            //Assert
            Assert.True(resp.Acked);
            Assert.Equal(10, indexer.Indexed.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.Contains(indexer.Indexed, e => e.Id == i.ToString());
            }
        }

        [Fact]
        public async Task ShouldDeleteFromIndex()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            var config = CreateConfig();

            var idsForDelete = new[] { "1", "2", "3" };

            var source = new TestEntitySource();
            var indexer = new TestEntityIndexer(source);
            
            var icfg = new IntegrationConfiguration
            {
                Configuration = config,
                EntityIndexerRegistrar = new ObjectSingletonRegistrar<IEntityIndexer>(indexer),
                EntityStorageRegistrar = new ObjectSingletonRegistrar<IOriginEntityStorage>(source),
                OverrideMqInitiatorRegistrar = new InputMessageEmulatorRegistrar()
            };

            serviceCollection.AddIndexerLogic(icfg);

            var services = serviceCollection.BuildServiceProvider();

            var msgEmulator = services.GetService<IInputMessageEmulator>();

            //Act
            var resp = await msgEmulator.Queue(new IndexingMsg
            {
                Delete = idsForDelete
            }, "bar");

            LogQueueRes(resp);

            //Assert
            Assert.True(resp.Acked);
            Assert.Equal(source.Count - idsForDelete.Length, indexer.Indexed.Count);

            foreach (var id in idsForDelete)
            {
                Assert.DoesNotContain(indexer.Indexed, e => e.Id == id);
            }
        }

        private void LogQueueRes(FakeMessageQueueProcResult resp)
        {
            _output.WriteLine("RESULT:");
            _output.WriteLine("");
            _output.WriteLine("Acked: " + resp.Acked);
            _output.WriteLine("Rejected: " + resp.Rejected);
            _output.WriteLine("RequeueFlag: " + resp.RequeueFlag);
            _output.WriteLine("Exception: " + resp.RejectionException);
        }

        private static IConfigurationRoot CreateConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{IndexerAppConst.RootConfigName}:{nameof(IndexerOptions.Index)}", "foo" },
                    { $"{IndexerAppConst.RootConfigName}:{nameof(IndexerOptions.Queue)}", "bar" },
                    { $"{IndexerAppConst.RootConfigName}:{nameof(IndexerOptions.BatchSize)}", "2" },
                    { $"{IndexerAppConst.RootConfigName}:{nameof(IndexerOptions.Db)}:{nameof(IndexerDbOptions.DbProviderName)}", "sqlite" },
                })
                .Build();
        }

        class TestEntityIndexer : IEntityIndexer
        {
            private readonly List<DbEntity> _indexed;
            public ReadOnlyCollection<DbEntity> Indexed {get;}

            public TestEntityIndexer(IEnumerable<DbEntity> initial = null)
            {
                _indexed = initial != null
                    ? new List<DbEntity>(initial)
                    : new List<DbEntity>();
                Indexed = new ReadOnlyCollection<DbEntity>(_indexed);
            }

            public Task IndexEntityBatchAsync(DbEntity[] docBatch)
            {
                _indexed.AddRange(docBatch);

                return Task.CompletedTask;
            }

            public Task RemoveEntitiesAsync(string[] ids)
            {
                _indexed.RemoveAll(e => ids.Contains(e.Id));
                return Task.CompletedTask;
            }

            public Task StartReindex()
            {
                _indexed.Clear();
                return Task.CompletedTask;
            }

            public Task EndReindex()
            {
                return Task.CompletedTask;
            }
        }

        class TestEntitySource : TestOriginEntityStorage
        {
            public TestEntitySource()
                :base(10)
            {
                
            }
        }
    }
}
