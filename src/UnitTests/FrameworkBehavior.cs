//using System.Collections.ObjectModel;
//using System.Linq;
//using LinqToDB.DataProvider.SQLite;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using MyLab.Indexer;
//using MyLab.Indexer.Services;
//using MyLab.Mq;
//using Xunit;

//namespace UnitTests
//{
//    public class FrameworkBehavior
//    {
//        [Fact]
//        public void ShouldSyncData()
//        {
//            //Arrange
//            var serviceCollection = new ServiceCollection();
//            var config = new ConfigurationBuilder()
//                .Build();

//            serviceCollection.AddIndexerLogic(
//                config,
//                "foo-queue",
//                new SQLiteDataProvider(),
//                new EntityConverter());

//            var services = serviceCollection.BuildServiceProvider();

//            var msgEmulator = services.GetService<IInputMessageEmulator>();

//            //Act
            

//            //Assert

//        }

//        class DataEntity : IIdentifiableEntity
//        {
//            public string Id { get; set; }
//            public string Val { get; set; }
//        }

//        class IndexDocumentEntity
//        {
//            public string Id { get; set; }
//            public string Val { get; set; }
//        }

//        class EntityConverter : IEntityConverter<DataEntity, IndexDocumentEntity>
//        {
//            public IndexDocumentEntity Convert(DataEntity entity)
//            {
//                return new IndexDocumentEntity
//                {
//                    Id = entity.Id,
//                    Val = entity.Val
//                };
//            }
//        }

//        class OriginEntityStorage : Collection<DataEntity>, IOriginEntityStorage<DataEntity>
//        {
//            public OriginEntityStorage(int count)
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    Add(new Entity { Id = i.ToString() });
//                }
//            }

//            public IQueryable<Entity> Query()
//            {
//                return this.AsQueryable();
//            }
//        }
//    }

//    class DbFixture
//    {

//    }
//}
