using System;

namespace TransitTracker
{
    public class VehiclePositionUpdated
    {
        public string VehicleId { get; set; }
        public string Label { get; set; }
        public Data.Models.VehicleStopStatus CurrentStatus { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}