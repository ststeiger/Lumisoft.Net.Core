using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Data.SQLite;

namespace System.Data.SQLite
{
    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    class SqlLite_Regex : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Regex.IsMatch(Convert.ToString(args[1]),Convert.ToString(args[0]),RegexOptions.IgnoreCase);
        }
    }
}
