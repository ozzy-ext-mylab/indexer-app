using System.Collections.Generic;
using System.Configuration;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.DataProvider.Oracle;

namespace MyLab.Indexer
{
    static class DataProviders
    {
        static readonly IDictionary<string, IDataProvider> Providers = new Dictionary<string, IDataProvider>
        {
            { "mysql", new MySqlDataProvider() },
            { "oracle", new OracleDataProvider() },
        };

        public static IDataProvider Get(string key)
        {
            return Providers.TryGetValue(key, out var provider)
                ? provider
                : throw new ConfigurationErrorsException("There is no configuration parameter InputQueue");
        }
    }
}