using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.DataProvider.Oracle;
using LinqToDB.DataProvider.SQLite;

namespace MyLab.Indexer
{
    public class SupportedDataProviders : ReadOnlyDictionary<string, IDataProvider>
    {
        public static readonly SupportedDataProviders Instance  = new SupportedDataProviders();
        SupportedDataProviders()
            :base(GetProviders())
        {
        }

        static IDictionary<string, IDataProvider> GetProviders()
        {
            return new Dictionary<string, IDataProvider>
            {
                {"sqlite", new SQLiteDataProvider()},
                {"mysql", new MySqlDataProvider()},
                {"oracle", new OracleDataProvider()}
            };
        }
    }
}
