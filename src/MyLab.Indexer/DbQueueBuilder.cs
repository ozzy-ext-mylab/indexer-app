//using System.Linq;

//namespace MyLab.Indexer
//{
//    static class DbQueueBuilder
//    {
//        public static string Build(string[] ids, IndexerDbOptions options)
//        {
//            var wrappedIds = ids.Select(id => options.IdFieldPrefix + id + options.IdFieldSuffix);
//            return options.SelectByIdsQuery.Replace("{ids}", string.Join(',', wrappedIds));
//        }

//        public static string Build(int skip, int count, IndexerDbOptions options)
//        {
//            return options.SelectPageQuery
//                .Replace("{offset}", skip.ToString())
//                .Replace("{limit}", count.ToString());
//        }
//    }
//}