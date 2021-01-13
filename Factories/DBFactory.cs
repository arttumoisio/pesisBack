namespace pesisBackend
{
    class DBFactory : IDBFactory
    {
        private SQLiteDBHandler _dbHandler = new SQLiteDBHandler();
        public IDBHandler dbHandler {
            get => _dbHandler;
            }
        public SQLiteQueries GetSQLiteQueries(){
            var sql = new SQLiteQueries(dbHandler);
            return sql;
        }   
        public ApuQueries GetApuQueries(){
            var apu = new ApuQueries(dbHandler);
            return apu;
        }
    }
}
