using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using MyLab.DbTest;
using MyLab.Indexer;
using MyLab.Indexer.Services;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class DefaultOriginEntityStorageBehavior : IClassFixture<TmpDbFixture>
    {
        private readonly TmpDbFixture _fxt;

        public DefaultOriginEntityStorageBehavior(TmpDbFixture fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _fxt = fxt;
        }

        [Fact]
        public async Task ShouldProvideEntities()
        {
            //Arrange
            var db = await _fxt.CreateDbAsync(new StrEntDbInitializer());

            var options = new IndexerOptions
            {
                Db = new IndexerDbOptions
                {
                    SelectQuery = "select * from test",
                    Fields = new []{nameof(StrTestEntity.Value) }
                }
            };

            var storage = new DefaultOriginEntityStorage(db, options);

            object fooVal = null, bazVal = null;

            //Act
            var entities = await storage.Query().ToArrayAsync();

            var fooEnt = entities.FirstOrDefault(e => e.Id == "foo");
            fooEnt?.ExtendedProperties.TryGetValue(nameof(StrTestEntity.Value), out fooVal);

            var bazEnt = entities.FirstOrDefault(e => e.Id == "baz");
            bazEnt?.ExtendedProperties.TryGetValue(nameof(StrTestEntity.Value), out bazVal);

            //Assert
            Assert.Equal(2, entities.Length);

            Assert.NotNull(fooEnt);
            Assert.Equal("bar", fooVal);
            Assert.NotNull(bazEnt);
            Assert.Equal("qux", bazVal);
        }

        [Fact]
        public async Task ShouldProvideEntitiesByStringId()
        {
            //Arrange
            var db = await _fxt.CreateDbAsync(new StrEntDbInitializer());

            var options = new IndexerOptions
            {
                Db = new IndexerDbOptions
                {
                    SelectQuery = "select * from test",
                    Fields = new[] { nameof(StrTestEntity.Value) }
                }
            };

            var storage = new DefaultOriginEntityStorage(db, options);

            //Act
            var found = await storage.Query().FirstOrDefaultAsync(e => e.Id == "foo");
            object foundVal = null;

            found?.ExtendedProperties.TryGetValue(nameof(StrTestEntity.Value), out foundVal);

            //Assert
            Assert.NotNull(found);
            Assert.Equal("bar", foundVal);
        }

        [Fact]
        public async Task ShouldProvideEntitiesByIntId()
        {
            //Arrange
            var db = await _fxt.CreateDbAsync(new IntEntDbInitializer());

            var options = new IndexerOptions
            {
                Db = new IndexerDbOptions
                {
                    SelectQuery = "select * from test",
                    Fields = new[] { nameof(StrTestEntity.Value) }
                }
            };

            var storage = new DefaultOriginEntityStorage(db, options);

            //Act
            var found = await storage.Query().FirstOrDefaultAsync(e => e.Id == "1");
            object foundVal = null;

            found?.ExtendedProperties.TryGetValue(nameof(StrTestEntity.Value), out foundVal);

            //Assert
            Assert.NotNull(found);
            Assert.Equal("2", foundVal);
        }

        [Fact]
        public async Task ShouldNotProvideEntitiesWhichExcludedByNestedQuery()
        {
            //Arrange
            var db = await _fxt.CreateDbAsync(new StrEntDbInitializer());

            var options = new IndexerOptions
            {
                Db = new IndexerDbOptions
                {
                    SelectQuery = "select * from test where id != 'foo'",
                    Fields = new[] { nameof(StrTestEntity.Value) }
                }
            };

            var storage = new DefaultOriginEntityStorage(db, options);

            //Act
            var found = await storage.Query().FirstOrDefaultAsync(e => e.Id == "foo");
            
            //Assert
            Assert.Null(found);
        }

        [Fact]
        public async Task ShouldSupportSyntheticFields()
        {
            //Arrange
            var db = await _fxt.CreateDbAsync(new StrEntDbInitializer());

            var options = new IndexerOptions
            {
                Db = new IndexerDbOptions
                {
                    SelectQuery = "select '1' as Id, 'foo' as Value",
                    Fields = new[] { nameof(StrTestEntity.Value) }
                }
            };

            var storage = new DefaultOriginEntityStorage(db, options);

            //Act
            var found = await storage.Query().FirstAsync();
            
            object foundVal = null;
            found?.ExtendedProperties.TryGetValue(nameof(StrTestEntity.Value), out foundVal);

            //Assert
            Assert.NotNull(found);
            Assert.Equal("foo", foundVal);
        }

        [Table("test")]
        class StrTestEntity
        {
            [Column, PrimaryKey]
            public string Id { get; set; }
            [Column]
            public string Value { get; set; }
        }

        class StrEntDbInitializer : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                var t = await dataConnection.CreateTableAsync<StrTestEntity>();

                await t.InsertAsync(() => new StrTestEntity
                {
                    Id = "foo",
                    Value = "bar"
                });
                
                await t.InsertAsync(() => new StrTestEntity
                {
                    Id = "baz",
                    Value = "qux"
                });
            }
        }

        [Table("test")]
        class IntTestEntity
        {
            [Column, PrimaryKey]
            public int Id { get; set; }
            [Column]
            public int Value { get; set; }
        }

        class IntEntDbInitializer : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                var t = await dataConnection.CreateTableAsync<IntTestEntity>();

                await t.InsertAsync(() => new IntTestEntity
                {
                    Id = 1,
                    Value = 2
                });

                await t.InsertAsync(() => new IntTestEntity
                {
                    Id = 3,
                    Value = 4
                });
            }
        }
    }
}
