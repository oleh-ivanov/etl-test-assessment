using CsvHelper.Configuration;
using ETL.Assessment.Application.Entities;

namespace ETL.Assessment.Application.Mappers
{
    public class CabDataMap : ClassMap<CabData>
    {
        public CabDataMap()
        {
            Map(m => m.TpepPickupDatetime).Name("tpep_pickup_datetime");
            Map(m => m.TpepDropoffDatetime).Name("tpep_dropoff_datetime");
            Map(m => m.PassengerCount).Name("passenger_count");
            Map(m => m.TripDistance).Name("trip_distance");
            Map(m => m.StoreAndFwdFlag).Name("store_and_fwd_flag");
            Map(m => m.PULocationID).Name("PULocationID");
            Map(m => m.DOLocationID).Name("DOLocationID");
            Map(m => m.FareAmount).Name("fare_amount");
            Map(m => m.TipAmount).Name("tip_amount");
        }
    }
}
