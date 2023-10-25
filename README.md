# ETL Test Assessment

ETL test assessment extracting and processing data from CSV with cab records.

### Build MS SQL Server

1. Run MS SQL in Docker:

```sh
$ make start_dependencies
```

This will create MS SQL container in Docker, note that the `appsettings.json` already has a correct connection string.

2. Publish Database:
   - Open Solution in Visual Studio
   - Rignt-click on ETL.Assessment.Database
   - Select Publish
   - Create connection based on connection string from the `appsettings.json`
   - Publish Database

### Run application

1. Restore dependencies

```sh
$ make install
```

2. Build projects. Use the command below or build with Visual Studio

```sh
$ make build
```

3. Run the `ConsoleApp` project using Visual Studio or `dotnet` command.

Note that the application expects two arguments: path to an input CSV file and path to a CSV file with duplicate records (file can be non-existent and will be created by the application), so when running the app, you will need to provide both arguments

When using `dotnet`

```sh
$ cd src/ETL.Assessment.ConsoleApp
$ dotnet run <path_to_input_file.csv> <path_to_duplicates_file.csv>
```


After execution 29889 rows added and 111 saved to the "duplicates.csv"
