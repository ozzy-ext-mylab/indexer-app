using System;

namespace MyLab.Indexer.Tools
{
    static class NewIndexNameBuilder
    {
        public static string Build(string baseIndexName) => baseIndexName + "-real-" + Guid.NewGuid().ToString("N");
    }
}