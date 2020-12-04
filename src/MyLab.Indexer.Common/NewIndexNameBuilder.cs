using System;

namespace MyLab.Indexer.Common
{
    public static class NewIndexNameBuilder
    {
        public static string Build(string baseIndexName) => baseIndexName + "-real-" + Guid.NewGuid().ToString("N");
    }
}