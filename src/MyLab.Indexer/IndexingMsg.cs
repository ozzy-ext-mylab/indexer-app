namespace MyLab.Indexer
{
    /// <summary>
    /// Indexing message 
    /// </summary>
    public class IndexingMsg
    {
        /// <summary>
        /// Identified array to update. Empty - update all.
        /// </summary>
        public string[] Update { get; set; }

        /// <summary>
        /// Identified array to delete. Empty - delete none.
        /// </summary>
        public string[] Delete{ get; set; }

    }
}