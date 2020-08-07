using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Mapping;
using Microsoft.Extensions.Options;
using MyLab.Db;

namespace MyLab.Indexer.Services
{
    class DefaultOriginEntityStorage : IOriginEntityStorage
    {
        private readonly IDbManager _db;
        private readonly IndexerOptions _options;
        private readonly MappingSchema _mappingSchema;

        public DefaultOriginEntityStorage(IDbManager db, IOptions<IndexerOptions> options)
            :this(db, options.Value)
        {
            
        }

        public DefaultOriginEntityStorage(IDbManager db, IndexerOptions options)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _mappingSchema = CreateMappingSchema(_db, _options.Db.IdField, _options.Db.Fields);
        }

        public IQueryable<DbEntity> Query()
        {
            var dc = _db.DoOnce();
            dc.MappingSchema = _mappingSchema;
            return dc.FromSql<DbEntity>(new RawSqlString(_options.Db.SelectQuery));
        }

        static MappingSchema CreateMappingSchema(IDbManager dbManager, string idFieldId, string[] fields)
        {
            var mappingSchema = new MappingSchema();

            var mb = mappingSchema.GetFluentMappingBuilder();
            var entB = mb.Entity<DbEntity>()
                .HasPrimaryKey(x => Sql.Property<string>(x, idFieldId));

            if (fields != null)
            {
                if (fields.Length > 0)
                {
                    var propB = entB.Property(x => Sql.Property<string>(x, fields[0])).IsNullable();
                    if (fields.Length > 1)
                    {
                        foreach (var f in fields.Skip(1))
                        {
                            propB = propB.Property(x => Sql.Property<string>(x, f)).IsNullable();
                        }
                    }
                }
            }

            return mappingSchema;
        }
    }
}