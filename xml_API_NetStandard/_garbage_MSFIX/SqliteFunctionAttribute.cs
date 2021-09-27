/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Microsof.Data.Sqlite
{
    using System;
    using System.Runtime.InteropServices;



    /// <summary>
    /// The type of user-defined function to declare
    /// </summary>
    public enum FunctionType
    {
        /// <summary>
        /// Scalar functions are designed to be called and return a result immediately.  Examples include ABS(), Upper(), Lower(), etc.
        /// </summary>
        Scalar = 0,
        /// <summary>
        /// Aggregate functions are designed to accumulate data until the end of a call and then return a result gleaned from the accumulated data.
        /// Examples include SUM(), COUNT(), AVG(), etc.
        /// </summary>
        Aggregate = 1,
        /// <summary>
        /// Collation sequences are used to sort textual data in a custom manner, and appear in an ORDER BY clause.  Typically text in an ORDER BY is
        /// sorted using a straight case-insensitive comparison function.  Custom collating sequences can be used to alter the behavior of text sorting
        /// in a user-defined manner.
        /// </summary>
        Collation = 2,
    }

    /// <summary>
    /// A simple custom attribute to enable us to easily find user-defined functions in
    /// the loaded assemblies and initialize them in SQLite as connections are made.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SqliteFunctionAttribute : Attribute
    {
        private string _name;
        private int _arguments;
        private FunctionType _functionType;
        internal Type _instanceType;

        /// <summary>
        /// Default constructor, initializes the internal variables for the function.
        /// </summary>
        public SqliteFunctionAttribute()
        {
            Name = "";
            Arguments = -1;
            FuncType = FunctionType.Scalar;
        }

        /// <summary>
        /// The function's name as it will be used in SQLite command text.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The number of arguments this function expects.  -1 if the number of arguments is variable.
        /// </summary>
        public int Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        /// <summary>
        /// The type of function this implementation will be.
        /// </summary>
        public FunctionType FuncType
        {
            get { return _functionType; }
            set { _functionType = value; }
        }
    }
}
