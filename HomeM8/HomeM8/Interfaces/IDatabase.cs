using SQLite;

namespace HomeM8
{
    public interface IDatabase
    {
        SQLiteConnection GetConnection(ConnectionType conType);
    }
}
