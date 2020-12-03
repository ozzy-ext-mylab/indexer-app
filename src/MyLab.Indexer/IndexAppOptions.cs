using Nest;

namespace MyLab.Indexer
{
    class IndexAppLogicOptions
    {
        public int PageSize { get; set; } = 100;
    }

    class IndexAppEsOptions
    {
        public string IndexName {get; set; }
    }

    class IndexAppDbOptions
    {
        public string Provider { get; set; }
        public string Query { get; set; }
    }

    class IndexAppMqOptions
    {
        public string Queue{ get; set; }
    }
}