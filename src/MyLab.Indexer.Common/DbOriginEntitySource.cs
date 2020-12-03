using System.Linq;
using LinqToDB;
using MyLab.Db;

namespace MyLab.Indexer.Common
{
    class DbOriginEntitySource : IOriginEntitySource
    {
        private readonly IDbManager _dbManager;
        private readonly string _sqlQuery;

        public DbOriginEntitySource(IDbManager dbManager, string sqlQuery)
        {
            _dbManager = dbManager;
            _sqlQuery = sqlQuery;
        }

        public IQueryable<OriginEntity> Query()
        {
            return _dbManager.DoOnce().FromSql<OriginEntity>(_sqlQuery);
        }
    }
}
