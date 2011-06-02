using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;

namespace Web.Repositories.Config
{
    public static class SqlCEDbHelper
    {
        private static string engineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";
        private static Type type;
        private static PropertyInfo localConnectionString;
        private static MethodInfo createDatabase;

        public static void CreateDatabaseFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            if (type == null)
            {
                type = Type.GetType(engineTypeName);
                localConnectionString = type.GetProperty("LocalConnectionString");
                createDatabase = type.GetMethod("CreateDatabase");
            }
            object engine = Activator.CreateInstance(type);
            localConnectionString
                .SetValue(engine, string.Format("Data Source='{0}';", filename), null);
            createDatabase
                .Invoke(engine, new object[0]);
        }
    }
}