namespace VSGBulgariaMarketplace.Persistence.Constants
{
    public static class DatabaseConnectionConstant
    {
        public const string DEFAULT_CONNECTION_STRING_NAME = "DefaultConnection";

        #region CREATE DATABASE CONSTANTS
        internal const string MASTER_CONNECTION_STRING_NAME = "MasterConnection";
        internal const string DB_NAME_PARAMETER_NAME = "name";
        internal const string CHECK_IF_DATABASE_EXISTS_SQL_QUERY = "SELECT * FROM sys.databases WHERE name = @name";
        internal const string CREATE_DATABASE_SQL_QUERY_TEMPLATE = "CREATE DATABASE {0}";
        #endregion
    }
}