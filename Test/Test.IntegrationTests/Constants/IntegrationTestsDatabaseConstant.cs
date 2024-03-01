namespace Test.IntegrationTests.Constants
{
    internal static class IntegrationTestsDatabaseConstant
    {
        internal const string INTEGRATION_TESTS_CONNECTION_STRING_NAME = "IntegrationTestsConnection";

        internal const string TABLE_COLUMNS_CSV_FILE_NAME = "table-columns.csv";
        internal const string VERSION_INFO_CSV_FILE_NAME = "version-info.csv";

        internal const string SELECT_TABLE_COLUMNS_SQL_QUERY_TEMPLATE =
            "SELECT t.TABLE_NAME AS TableName, " +
                    "c.COLUMN_NAME AS ColumnName, " +
                    "c.DATA_TYPE AS DataType, " +
                    "tc.CONSTRAINT_TYPE AS ConstraintType, " +
                    "tc.CONSTRAINT_NAME AS ConstraintName " +
            "FROM INFORMATION_SCHEMA.TABLES t " +
            "INNER JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME " +
            "LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON c.TABLE_NAME = ccu.TABLE_NAME " +
            "AND c.COLUMN_NAME = ccu.COLUMN_NAME " +
            "LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " +
            "WHERE t.TABLE_TYPE = 'BASE TABLE' AND t.TABLE_CATALOG = '{0}' " +
            "ORDER BY t.TABLE_NAME, c.COLUMN_NAME;";

        internal const string SELECT_VERSION_INFO_SQL_QUERY = "SELECT [Version], [Description] FROM VersionInfo";
    }
}
