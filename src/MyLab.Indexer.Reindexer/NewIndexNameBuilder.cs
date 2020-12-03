using System;

namespace MyLab.Indexer.Reindexer
{
    static class NewIndexNameBuilder
    {
        public static string Build(string baseIndexName) => baseIndexName + "-real-" + Guid.NewGuid().ToString("N");
    }
}