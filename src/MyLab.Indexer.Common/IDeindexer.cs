using System.Threading.Tasks;

namespace MyLab.Indexer.Common
{
    public interface IDeindexer
    {
        Task DeindexAsync(string[] ids);
    }
}