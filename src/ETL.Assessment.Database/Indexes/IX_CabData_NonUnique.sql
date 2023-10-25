CREATE INDEX [IX_CabData_NonUnique]
	ON [dbo].[CabData]
	(tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count);
