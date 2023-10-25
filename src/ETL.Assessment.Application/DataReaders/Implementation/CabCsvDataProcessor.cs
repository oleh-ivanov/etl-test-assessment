using CsvHelper;
using CsvHelper.Configuration;
using ETL.Assessment.Application.Entities;
using ETL.Assessment.Application.Mappers;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ETL.Assessment.Application.DataReaders.Implementation
{
    public class CabCsvDataProcessor
    {
        public void Run(string csvFilePath, string duplicatesCsvFilePath, string connectionString)
        {
            int batchSize = 50000;

            using var connection = new SqlConnection(connectionString);
            using var streamReader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            });

            csv.Context.RegisterClassMap<CabDataMap>();
            connection.Open();

            var records = new List<CabData>(batchSize);

            while (csv.Read())
            {
                var record = csv.GetRecord<CabData>();
                records.Add(record);

                if (records.Count >= batchSize)
                {
                    BulkInsertIntoDatabase(connection, records);
                    records.Clear();
                }
            }

            // Insert any remaining records
            if (records.Count > 0)
            {
                BulkInsertIntoDatabase(connection, records);
            }
            Console.WriteLine("Cab records were inserted to the database.");

            DeleteDuplicates(duplicatesCsvFilePath, connection);
            ConvertStoreAndFwdFlag(connection);

            connection.Close();
        }

        private static void BulkInsertIntoDatabase(SqlConnection connection, List<CabData> records)
        {
            using var bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "CabData";

            bulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
            bulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
            bulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
            bulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
            bulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
            bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
            bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
            bulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
            bulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");

            var dataTable = ConvertListToDataTable(records);

            bulkCopy.WriteToServer(dataTable);
        }

        private static DataTable ConvertListToDataTable(List<CabData> records)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
            dataTable.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
            dataTable.Columns.Add("passenger_count", typeof(int));
            dataTable.Columns.Add("trip_distance", typeof(double));
            dataTable.Columns.Add("store_and_fwd_flag", typeof(string));
            dataTable.Columns.Add("PULocationID", typeof(int));
            dataTable.Columns.Add("DOLocationID", typeof(int));
            dataTable.Columns.Add("fare_amount", typeof(double));
            dataTable.Columns.Add("tip_amount", typeof(double));

            foreach (var record in records)
            {
                dataTable.Rows.Add(
                    record.TpepPickupDatetime,
                    record.TpepDropoffDatetime,
                    record.PassengerCount,
                    record.TripDistance,
                    record.StoreAndFwdFlag,
                    record.PULocationID,
                    record.DOLocationID,
                    record.FareAmount,
                    record.TipAmount);
            }

            return dataTable;
        }

        private static void DeleteDuplicates(string csvFilePath, SqlConnection connection)
        {
            var duplicates = new List<CabData>();

            var duplicateRecords = new Dictionary<string, CabData>();

            using (var command = new SqlCommand("SELECT * FROM CabData", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int? passengerCount = reader["passenger_count"] != DBNull.Value ? (int?)reader["passenger_count"] : null;

                        var record = new CabData
                        {
                            Id = (int)reader["Id"],
                            TpepPickupDatetime = (DateTime)reader["tpep_pickup_datetime"],
                            TpepDropoffDatetime = (DateTime)reader["tpep_dropoff_datetime"],
                            PassengerCount = passengerCount,
                        };

                        string key = $"{record.TpepPickupDatetime}_{record.TpepDropoffDatetime}_{record.PassengerCount}";

                        if (duplicateRecords.ContainsKey(key))
                        {
                            duplicates.Add(record);
                        }
                        else
                        {
                            duplicateRecords[key] = record;
                        }
                    }
                }
            }

            Console.WriteLine($"Identified {duplicates.Count} duplicates in the CabData.");

            if (duplicates.Count > 0)
            {
                using (var writer = new StreamWriter(csvFilePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(duplicates);
                }
            }

            foreach (var duplicate in duplicates)
            {
                using (var deleteCommand = new SqlCommand("DELETE FROM CabData WHERE Id = @Id", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", duplicate.Id);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Removed duplicates from CabData.");
        }

        private static void ConvertStoreAndFwdFlag(SqlConnection connection)
        {
            var updateSql = @"
        UPDATE dbo.CabData
        SET store_and_fwd_flag = CASE
            WHEN store_and_fwd_flag = 'N' THEN 'No'
            WHEN store_and_fwd_flag = 'Y' THEN 'Yes'
            ELSE store_and_fwd_flag
        END;
    ";
            using var command = new SqlCommand(updateSql, connection);

            command.ExecuteNonQuery();
            Console.WriteLine("Updated store_and_fwd_flag values mapping to Yes and No.");
        }
    }
}
