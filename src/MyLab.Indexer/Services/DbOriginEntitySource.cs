using System.Linq;
using LinqToDB;
using Microsoft.Extensions.Options;
using MyLab.Db;
using MyLab.Indexer.Tools;

namespace MyLab.Indexer.Services
{
    class DbOriginEntitySource : IOriginEntitySource
    {
        private readonly IDbManager _dbManager;
        private readonly IndexAppDbOptions _dbOpt;

        public DbOriginEntitySource(IDbManager dbManager, IOptions<IndexAppDbOptions> dbOpt)
        {
            _dbManager = dbManager;
            _dbOpt = dbOpt.Value;
        }

        public IQueryable<OriginEntity> Query()
        {
            return _dbManager.DoOnce().FromSql<OriginEntity>(_dbOpt.Query);
        }
    }
}
