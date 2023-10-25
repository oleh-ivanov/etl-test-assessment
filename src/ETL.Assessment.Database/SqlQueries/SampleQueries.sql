-- Find out which PULocationId (Pick-up location ID) has the highest tip_amount on average.
-- SELECT PULocationId, AVG(tip_amount) as avg_tip_amount
-- FROM [dbo].[CabData]
-- GROUP BY PULocationId
-- ORDER BY avg_tip_amount DESC

-- Find the top 100 longest fares in terms of trip_distance.
-- SELECT TOP 100 *
-- FROM [dbo].[CabData]
-- ORDER BY trip_distance DESC;

-- Find the top 100 longest fares in terms of time spent traveling.
-- SELECT TOP 100 *, DATEDIFF(millisecond, tpep_pickup_datetime, tpep_dropoff_datetime) as trip_duration
-- FROM [dbo].[CabData]
-- ORDER BY trip_duration DESC;

-- Search, where part of the conditions is PULocationId.
SELECT *
FROM [dbo.].[CabData]
WHERE PULocationId = 93 AND passenger_count < 5;
