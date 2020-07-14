using System.Collections.Generic;
using MyLab.Indexer.Services;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class IndexDocumentBuilderBehavior
    {
        private readonly ITestOutputHelper _output;

        public IndexDocumentBuilderBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldAddId()
        {
            //Arrange
            var dbEntity = new DbEntity
            {
                Id = "foo"
            };
            var builder = new IndexDocumentBuilder(dbEntity);

            //Act
            var xml = builder.BuildXml();
            var json = builder.BuildJson();

            _output.WriteLine(builder.BuildJson(Formatting.Indented));

            //Assert
            Assert.Equal("foo", xml.Root.Element(nameof(DbEntity.Id)).Value);
            Assert.Equal("{\"Id\":\"foo\"}", json);
        }

        [Fact]
        public void ShouldAddProperty()
        {
            //Arrange
            var dbEntity = new DbEntity
            {
                Id = "foo",
                ExtendedProperties = new Dictionary<string, object> 
                { 
                    { "Bar", "baz" }
                }
            };
            var builder = new IndexDocumentBuilder(dbEntity);

            //Act
            var xml = builder.BuildXml();
            var json = builder.BuildJson();
            
            _output.WriteLine(builder.BuildJson(Formatting.Indented));

            var fooProp = xml.Root?.Element("Bar");

            //Assert
            Assert.NotNull(fooProp);
            Assert.Equal("baz", fooProp.Value);
            Assert.Equal("{\"Id\":\"foo\",\"Bar\":\"baz\"}", json);
        }

        [Fact]
        public void ShouldIgnoreNullProperty()
        {
            //Arrange
            var dbEntity = new DbEntity
            {
                ExtendedProperties = new Dictionary<string, object>
                {
                    { "foo", null }
                }
            };
            var builder = new IndexDocumentBuilder(dbEntity);

            //Act
            var xml = builder.BuildXml();
            
            var fooProp = xml.Root?.Element("foo");

            //Assert
            Assert.Null(fooProp);
        }
    }
}
