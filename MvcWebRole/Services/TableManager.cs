using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace MvcWebRole.Services
{
    public class TableManager
    {
        private static CloudTable GetTable(string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["db"].ConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();

            return table;
        }

        public static async Task Add<T>(string tableName, T entity) where T : ITableEntity
        {
            var table = GetTable(tableName);
            var insertOperation = TableOperation.Insert(entity);

            await Task.Factory.FromAsync(table.BeginExecute(insertOperation, null, null), 
                ar => 
                    {
                        if (ar.IsCompleted)
                        {
                            table.EndExecute(ar);
                        }
                    });
        }

        public static IEnumerable<T> Get<T>(string tableName, Expression<Func<T, bool>> filter) where T : ITableEntity, new()
        {
            var table = GetTable(tableName);

            var builder = new ExpressionBuilder();
            var filterQuery = builder.ProcessExpression(filter);
            
            var query = new TableQuery<T>();
            query.Where(filterQuery);

            return table.ExecuteQuery(query);
        }
    }
}