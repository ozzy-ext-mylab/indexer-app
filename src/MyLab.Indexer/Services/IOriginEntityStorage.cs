using System.Linq;
using System.Threading.Tasks;

namespace MyLab.Indexer.Services
{
    /// <summary>
    /// Determines origin entity storage
    /// </summary>
    public interface IOriginEntityStorage
    {
        /// <summary>
        /// Provides <see cref="IQueryable{T}"/> for origin entities
        /// </summary>
        IQueryable<DbEntity> Query();
    }
}