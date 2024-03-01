namespace Test.IntegrationTests.Helpers
{
    using CsvHelper.Configuration;
    using CsvHelper;

    using System.Globalization;

    internal static class ReadCsvHelper
    {
        internal async static Task<List<T>> GetListFromCsvFileAsync<T>(string filePath, bool hasHeaderRecord)
        {
            CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord
            };

            using StreamReader streamReader = new(filePath);
            using CsvReader csvReader = new(streamReader, configuration);
            var records = await csvReader.GetRecordsAsync<T>().ToListAsync();

            return records;
        }
    }
}
