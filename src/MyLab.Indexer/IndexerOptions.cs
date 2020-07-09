namespace MyLab.Indexer
{
    /// <summary>
    /// Indexer app options
    /// </summary>
    public class IndexerOptions
    {
        /// <summary>
        /// Target index name
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Queue to listening
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// Indexing batch size
        /// </summary>
        public int BatchSize { get; set; } = 10;

        /// <summary>
        /// Data base related options
        /// </summary>
        public IndexerDbOptions Db { get; set; }
    }

    /// <summary>
    /// Contains data base related options
    /// </summary>
    public class IndexerDbOptions
    {
        /// <summary>
        /// DB provider name
        /// </summary>
        /// <remarks>
        /// To get supported values: mysql, oracle, sqlite
        /// </remarks>
        public string DbProviderName { get; set; }

        /// <summary>
        /// Entities selection query
        /// </summary>
        /// <remarks>
        /// select * from t
        /// </remarks>
        public string SelectQuery { get; set; }

        /// <summary>
        /// Identifier field name. `Id` by default. 
        /// </summary>
        public string IdField { get; set; } = "Id";

        /// <summary>
        /// Fields to index
        /// </summary>
        public string[] Fields { get; set; }

    }
}