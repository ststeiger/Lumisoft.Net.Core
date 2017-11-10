
using System.Text.RegularExpressions;


//using SQLiteFunction = Microsof.Data.Sqlite.SqliteFunction;
//using SQLiteFunctionAttribute = Microsof.Data.Sqlite.SqliteFunctionAttribute;
//using FunctionType = Microsof.Data.Sqlite.FunctionType;

using SQLiteFunction = Mono.Data.Sqlite.SqliteFunction;
using SQLiteFunctionAttribute = Mono.Data.Sqlite.SqliteFunctionAttribute;
using FunctionType = Mono.Data.Sqlite.FunctionType;


namespace System.Data.SQLite
{

    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    class SqlLite_Regex : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Regex.IsMatch(System.Convert.ToString(args[1]), System.Convert.ToString(args[0]), RegexOptions.IgnoreCase);
        }
    }

}
