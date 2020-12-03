using System.Collections.Generic;
using LinqToDB.Mapping;

namespace MyLab.Indexer.Common
{
    public class OriginEntity
    {
        [PrimaryKey]
        public string Id { get; set; }

        [DynamicColumnsStore]
        public IDictionary<string, object> Properties { get; set; }
    }
}