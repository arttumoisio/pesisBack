namespace pesisBackend
{
    interface IDBFactory
    {
        IDBHandler dbHandler {get;}
        SQLiteQueries GetSQLiteQueries();
        ApuQueries GetApuQueries();
    }
}
