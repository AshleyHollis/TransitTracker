using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransitTracker.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string Label { get; set; }
        public VehicleStopStatus CurrentStatus { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }     
        public DateTime TimeStamp { get; set; }
    }

    public enum VehicleStopStatus
    {
        IncomingAt,
        StoppedAt,
        InTransitTo,
    }
}