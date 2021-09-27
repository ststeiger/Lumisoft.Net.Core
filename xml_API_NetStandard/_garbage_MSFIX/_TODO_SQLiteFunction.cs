
namespace Microsof.Data.Sqlite
{

    public abstract class SqliteFunction
    {
        public virtual object Invoke(object[] args)
        {
            throw new System.NotImplementedException("SQLiteFunction");
            // return null;
        }
    }

}
