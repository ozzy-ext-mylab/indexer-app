namespace MyLab.Indexer.Services
{
    class IndexingMsg
    {
        public bool ReindexRequired { get; set; }
        public string[] IndexList { get; set; }
        public string[] DeindexList { get; set; }
    }
}