using System.Collections.Generic;
using LinqToDB.Mapping;

namespace MyLab.Indexer.Services
{
    public class DbEntity
    {
        [PrimaryKey]
        public string Id { get; set; }

        [DynamicColumnsStore]
        public IDictionary<string, object> ExtendedProperties { get; set; }
    }
}