namespace Test.IntegrationTests.Helpers
{
    using CsvHelper.Configuration;
    using CsvHelper;

    using System.Globalization;

    internal static class ReadCsvHelper
    {
        internal static List<T> GetListFromCsvFile<T>(string filePath, bool hasHeaderRecord)
        {
            CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord
            };

            using StreamReader streamReader = new(filePath);
            using CsvReader csvReader = new(streamReader, configuration);
            List<T> records = csvReader.GetRecordsAsync<T>()
                                       .ToBlockingEnumerable()
                                       .ToList();

            return records;
        }
    }
}
