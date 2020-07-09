using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Indexer;
using MyLab.Indexer.Services;
using Xunit;

namespace UnitTests
{
    public class OriginEntityProviderBehavior
    {
        [Theory]
        [MemberData(nameof(GetEmptyIdArrayCases))]
        public async Task ShouldProvideAllWhenIdsNotSpecified(string[] ids)
        {
            //Arrange
            var src = new TestOriginEntityStorage(4);
            var opt = new IndexerOptions
            {
                BatchSize = 2
            };
            var provider = new OriginEntityProvider(src, opt);
            
            var providedBatches = new List<DbEntity[]>(); 
            var providedItems = new List<DbEntity>(); 

            //Act
            await foreach (var entArr in provider.ProvideEntities(ids))
            {
                providedBatches.Add(entArr);
                providedItems.AddRange(entArr);
            }

            //Assert
            Assert.Equal(2, providedBatches.Count);
            Assert.Equal(new []{"0", "1"}, providedBatches[0].Select(e => e.Id));
            Assert.Equal(src.Select(e => e.Id), providedItems.Select(e => e.Id));
        }

        [Fact]
        public async Task ShouldProvideWhenSpecified()
        {
            //Arrange
            var src = new TestOriginEntityStorage(4);
            var opt = new IndexerOptions
            {
                BatchSize = 2
            };
            var provider = new OriginEntityProvider(src, opt);

            var requiredIds = new [] {"1", "2"};

            var providedBatches = new List<DbEntity[]>();

            //Act
            await foreach (var entArr in provider.ProvideEntities(requiredIds))
            {
                providedBatches.Add(entArr);
            }

            //Assert
            Assert.Single(providedBatches);
            Assert.Equal(requiredIds, providedBatches[0].Select(e => e.Id));
        }

        public static IEnumerable<object[]> GetEmptyIdArrayCases()
        {
            yield return new object[]{ null };
            yield return new object[]{ new string[0]  };
        }
    }
}
