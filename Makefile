install:
	dotnet restore
build:
	dotnet build src/ETL.Assessment.Application
	dotnet build src/ETL.Assessment.ConsoleApp
start_dependencies:
	docker-compose run start_dependencies
