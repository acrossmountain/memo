using Memo.Models;
using System.Diagnostics;

namespace Memo.Helpers
{
    public static class DatabaseHelper
    {
        public static IFreeSql Build()
        {
            IFreeSql freeSql = new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.Sqlite, @"Data Source=memo.db")
                    .UseMonitorCommand(cmd => Debug.WriteLine($"Sql: {cmd.CommandText}"))
                    .UseAutoSyncStructure(true)
                    .Build();

            freeSql.CodeFirst.SyncStructure<TodoModel>();
            freeSql.CodeFirst.SyncStructure<MemoModel>();
            freeSql.CodeFirst.SyncStructure<UserModel>();

            return freeSql;
        }
    }
}
