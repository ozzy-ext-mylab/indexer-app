using System;
using System.Threading.Tasks;
using MyLab.Elastic.Test;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class NestClientBehavior : IClassFixture<EsFixture>
    {
        private readonly EsFixture _esFixture;

        public NestClientBehavior(EsFixture esFixture, ITestOutputHelper outputHelper)
        {
            _esFixture = esFixture;
            _esFixture.Output = outputHelper;
        }

        [Fact]
        public async Task ShouldDetectIndexAbsence()
        {
            //Arrange


            //Act
            var res = await _esFixture.Manager.Client.Indices.ExistsAsync(Guid.NewGuid().ToString("N"));

            //Assert

            Assert.False(res.Exists);
        }
    }
}
