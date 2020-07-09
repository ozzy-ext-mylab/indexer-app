namespace MyLab.Indexer
{
    /// <summary>
    /// Indexing message 
    /// </summary>
    public class IndexingMsg
    {
        /// <summary>
        /// Indicates that need to reindex all entities. 'Update' property is ignored.
        /// </summary>
        public bool Reindex { get; set; }

        /// <summary>
        /// Identified array to update. Empty - update none.
        /// </summary>
        public string[] Update { get; set; }

        /// <summary>
        /// Identified array to delete. Empty - delete none.
        /// </summary>
        public string[] Delete{ get; set; }

    }
}