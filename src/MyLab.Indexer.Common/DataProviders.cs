using System.Collections.Generic;
using System.Configuration;
using LinqToDB;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.DataProvider.Oracle;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Extensions.Configuration;

namespace MyLab.Indexer.Common
{
    public static class DataProviders
    {
        static readonly IDictionary<string, IDataProvider> Providers = new Dictionary<string, IDataProvider>
        {
            { "sqlite", new SQLiteDataProvider(ProviderName.SQLite) },
            { "mysql", new MySqlDataProvider(ProviderName.MySql) },
            { "oracle", new OracleDataProvider(ProviderName.Oracle) },
        };

        public static IDataProvider Get(string key)
        {
            return Providers.TryGetValue(key, out var provider)
                ? provider
                : throw new ConfigurationErrorsException("There is no configuration parameter InputQueue");
        }

        public static IDataProvider Get<TOptions>(IConfigurationSection config)
            where TOptions : IIndexingOptions, new()
        {
            var options = new TOptions();

            config.Bind(options);

            return Get(options.DataProvider);
        }
    }
}