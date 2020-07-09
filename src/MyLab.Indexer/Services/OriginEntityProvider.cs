using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Options;

namespace MyLab.Indexer.Services
{
    class OriginEntityProvider 
    {
        private readonly IOriginEntityStorage _storage;
        private readonly IndexerOptions _options;

        public OriginEntityProvider(IOriginEntityStorage storage, IOptions<IndexerOptions> options)
            : this(storage, options.Value)
        {
        }


        public OriginEntityProvider(IOriginEntityStorage storage, IndexerOptions options)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IAsyncEnumerable<DbEntity[]> ProvideEntities(string[] ids)
        {
            return new OriginEntityEnumerable(_storage, _options.BatchSize, ids);
        }

        public IAsyncEnumerable<DbEntity[]> ProvideAllEntities()
        {
            return new OriginEntityEnumerable(_storage, _options.BatchSize);
        }

        class OriginEntityEnumerable : IAsyncEnumerable<DbEntity[]>
        {
            private readonly IOriginEntityStorage _storage;
            private readonly string[] _ids;
            private readonly int _batchSize;

            public OriginEntityEnumerable(IOriginEntityStorage storage, int batchSize, string[] ids = null)
            {
                _storage = storage ?? throw new ArgumentNullException(nameof(storage));
                _ids = ids;
                _batchSize = batchSize;
            }

            public IAsyncEnumerator<DbEntity[]> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
            {
                return new OriginEntityEnumerator(_storage, _ids, _batchSize, cancellationToken);
            }
        }

        class OriginEntityEnumerator : IAsyncEnumerator<DbEntity[]>
        {
            private readonly IOriginEntityStorage _storage;
            private readonly string[] _ids;
            private readonly int _batchSize;
            private readonly CancellationToken _cancellationToken;

            private int _pageIndex;

            public DbEntity[] Current { get; private set; }

            public OriginEntityEnumerator(IOriginEntityStorage storage, string[] ids, int batchSize, CancellationToken cancellationToken)
            {
                _storage = storage ?? throw new ArgumentNullException(nameof(storage));
                _ids = ids;
                _batchSize = batchSize;
                _cancellationToken = cancellationToken;
            }

            public ValueTask DisposeAsync()
            {
                _pageIndex = 0;

                return new ValueTask(Task.CompletedTask);
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                IQueryable<DbEntity> q = _storage.Query();

                if (_ids != null && _ids.Length > 1)
                {
                    var pageIds =_ids
                        .Skip(_pageIndex * _batchSize)
                        .Take(_batchSize)
                        .ToArray();

                    if (pageIds.Length == 0) return false;

                    q = q.Where(itm => pageIds.Contains(itm.Id));
                }
                else
                {
                    q = q
                        .Skip(_pageIndex * _batchSize)
                        .Take(_batchSize);
                }

                _pageIndex++;

                Current = await q.ToArrayAsync(_cancellationToken);

                return Current.Length != 0;
            }
        }

    }
}