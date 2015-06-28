using System.Diagnostics;
using Akka.Actor;
using TransitTracker.Data;

namespace TransitTracker
{
    internal class DatabaseActor : TypedActor, IHandle<VehiclePositionUpdated>
    {
        public async void Handle(VehiclePositionUpdated message)
        {
            using (var transitTrackerContext = new TransitTrackerContext())
            {
                var vehiclePositionsEntity = new Data.Models.VehiclePosition
                {
                    VehicleId = message.VehicleId,
                    Label = message.Label,
                    CurrentStatus = message.CurrentStatus,
                    Latitude = message.Latitude,
                    Longitude = message.Longitude,
                    TimeStamp = message.TimeStamp
                };

                Debug.WriteLine("Adding vehiclePositionsEntity to database");
                transitTrackerContext.VehiclePositions.Add(vehiclePositionsEntity);
                await transitTrackerContext.SaveChangesAsync();
                Debug.WriteLine("Added vehiclePositionsEntity to database");
            }
        }
    }
}