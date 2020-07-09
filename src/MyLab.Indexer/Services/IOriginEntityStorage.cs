using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Mapping;
using Microsoft.Extensions.Options;
using MyLab.Db;

namespace MyLab.Indexer.Services
{
    /// <summary>
    /// Determines origin entity storage
    /// </summary>
    public interface IOriginEntityStorage
    {
        ///// <summary>
        ///// Provides <see cref="IQueryable{T}"/> for origin entities
        ///// </summary>
        //IQueryable<DbEntity> Query(string[] ids);

        ///// <summary>
        ///// Provides <see cref="IQueryable{T}"/> for origin entities
        ///// </summary>
        //IQueryable<DbEntity> Query(int skip, int count);

        /// <summary>
        /// Provides <see cref="IQueryable{T}"/> for origin entities
        /// </summary>
        IQueryable<DbEntity> Query();
    }

    public class DbEntity
    {
        [PrimaryKey]
        public string Id { get; set; }

        [DynamicColumnsStore]
        public IDictionary<string, object> ExtendedProperties { get; set; }
    }

    class DefaultOriginEntityStorage : IOriginEntityStorage
    {
        private readonly IDbManager _db;
        private readonly IndexerOptions _options;

        public DefaultOriginEntityStorage(IDbManager db, IOptions<IndexerOptions> options)
            :this(db, options.Value)
        {
            
        }

        public DefaultOriginEntityStorage(IDbManager db, IndexerOptions options)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        //public IQueryable<DbEntity> Query(string[] ids)
        //{
        //    var query = _db.DoOnce().FromSql<DbEntity>(new RawSqlString())
        //}

        //public IQueryable<DbEntity> Query(int skip, int count)
        //{
        //    throw new NotImplementedException();
        //}

        public IQueryable<DbEntity> Query()
        {
            var dc = _db.DoOnce();

            var mb = dc.MappingSchema.GetFluentMappingBuilder();
            var entB = mb.Entity<DbEntity>()
                .HasPrimaryKey(x => Sql.Property<string>(x, _options.Db.IdField));

            var fields = _options.Db.Fields;

            if (fields != null)
            {
                if (fields.Length > 0)
                {
                    var propB = entB.Property(x => Sql.Property<string>(x, fields[0])).IsNullable();
                    if (fields.Length > 1)
                    {
                        foreach (var f in _options.Db.Fields.Skip(1))
                        {
                            propB = propB.Property(x => Sql.Property<string>(x, f)).IsNullable();
                        }
                    }
                }
            }

            return dc.FromSql<DbEntity>(new RawSqlString(_options.Db.SelectQuery));
        }
    }
}