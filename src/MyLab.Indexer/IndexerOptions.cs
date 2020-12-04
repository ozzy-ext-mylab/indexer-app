using MyLab.Indexer.Common;

namespace MyLab.Indexer
{
    public class IndexerOptions : IIndexingOptions
    {
        /// <summary>
        /// Data provider name: 'sqlite', 'mysql' or 'oracle' 
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// SQL query which get properties for indexing
        /// </summary>
        public string SqlQuery { get; set; }

        /// <summary>
        /// Index alias name
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Size of the data page that will be requested from the database at a time
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Queue for listening a indexing messages
        /// </summary>
        public string Queue { get; set; }
    }
}
