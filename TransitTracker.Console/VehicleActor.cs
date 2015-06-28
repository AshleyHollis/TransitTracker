using System;
using System.Diagnostics;
using Akka.Actor;

namespace TransitTracker
{
    public class VehicleActor : TypedActor, IHandle<Data.Models.VehiclePosition>
    {
        private IActorRef _databaseActor;
        private IActorRef _vechiclePositionPostActor;

        private string _vehicleId;
        private float _lastLat;
        private float _lastLong;
        private DateTime _lastUpdate;

        public VehicleActor(string vehicleId, IActorRef databaseActor, IActorRef vechiclePositionPostActor)
        {
            _vehicleId = vehicleId;
            _databaseActor = databaseActor;
            _vechiclePositionPostActor = vechiclePositionPostActor;

        }

        public void Handle(Data.Models.VehiclePosition message)
        {
            _vehicleId = message.VehicleId;            

            if (_vehicleId != null
                && message.TimeStamp != _lastUpdate
                && ((_lastLat != 0 && _lastLat != message.Latitude) || (_lastLong != 0 && _lastLong != message.Longitude)))
            {
                var vehiclePositionUpdatedMessage = new VehiclePositionUpdated
                {
                    VehicleId = message.VehicleId,
                    Label = message.Label,
                    CurrentStatus = message.CurrentStatus,
                    Latitude = message.Latitude,
                    Longitude = message.Longitude,
                    TimeStamp = message.TimeStamp
                };

                _vechiclePositionPostActor.Tell(vehiclePositionUpdatedMessage);
                _databaseActor.Tell(vehiclePositionUpdatedMessage);
            }

            _lastLat = message.Latitude;
            _lastLong = message.Longitude;
            _lastUpdate = message.TimeStamp;

            //Console.WriteLine(string.Join("\t", message.vehicle.id, message.vehicle.label, message.current_status, message.position.latitude + ", " + message.position.longitude, UnixTimeStampToDateTime(message.timestamp).TimeOfDay));
        }
    }
}