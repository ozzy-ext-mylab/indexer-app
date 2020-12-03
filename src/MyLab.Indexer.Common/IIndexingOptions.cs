namespace MyLab.Indexer.Common
{
    public interface IIndexingOptions
    {
        /// <summary>
        /// Data provider name: 'sqlite', 'mysql' or 'oracle' 
        /// </summary>
        public string DataProvider { get; }

        /// <summary>
        /// SQL query which get properties for indexing
        /// </summary>
        public string SqlQuery { get; }
    }
}
