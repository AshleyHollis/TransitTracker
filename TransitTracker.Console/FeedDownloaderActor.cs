using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Akka.Actor;
using Akka.Routing;
using ProtoBuf;
using transit_realtime;
using TransitTracker.Data.Models;

namespace TransitTracker
{
    public class FeedDownloaderActor : TypedActor, IHandle<DownloadFeed>
    {
        private IActorRef _databaseActor;
        private IActorRef _vechiclePositionPostActor;
        private Dictionary<string, IActorRef> createdActors = new Dictionary<string, IActorRef>();

        public FeedDownloaderActor(IActorRef databaseActor, IActorRef vechiclePositionPostActor)
        {
            _databaseActor = databaseActor;
            _vechiclePositionPostActor = vechiclePositionPostActor;
        }

        public void Handle(DownloadFeed message)
        {
            {
                while (true)
                {
                    var req = WebRequest.Create(message.URL);
                    var feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());

                    foreach (var entity in feed.entity
                        .Where(a => !a.is_deleted && a.vehicle != null))
                        //.Take(20))
                        //.Where(b => b.vehicle.vehicle.label == "1"))
                    {
                        var vehiclePositionMessage = new Data.Models.VehiclePosition
                        {
                            VehicleId = entity.vehicle.vehicle.id,
                            Label = entity.vehicle.vehicle.label,
                            CurrentStatus = (VehicleStopStatus) entity.vehicle.current_status,
                            Latitude = entity.vehicle.position.latitude,
                            Longitude = entity.vehicle.position.longitude,
                            TimeStamp = Helpers.UnixTimeStampToDateTime(entity.vehicle.timestamp)
                        };

                        var vehicleId = entity.vehicle.vehicle.id;
                        if (!createdActors.ContainsKey(vehicleId))
                        {
                            var vehicleActorProps = Props.Create<VehicleActor>(vehicleId, _databaseActor, _vechiclePositionPostActor);
                            var vehicleActor = Context.ActorOf(vehicleActorProps, vehicleId);
                            createdActors.Add(vehicleId, vehicleActor);
                            //vehicleActor.Tell(vehiclePositionMessage);
                        }
                        else
                        {                          
                            var vehicleActor = Context.ActorSelection($"/user/FeedDownloaderActor/{vehicleId}");
                            vehicleActor.Tell(vehiclePositionMessage);
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Console.WriteLine("Complete...");
                    Console.WriteLine();
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    //Console.ReadLine();
                }
            }
        }
    }

    public class VehiclePositionRouterActor : ReceiveActor
    {
        public VehiclePositionRouterActor()
        {
            Receive<Data.Models.VehiclePosition>(vehiclePosition =>
            {
                Context.ActorSelection("/user/vehiclePositions").Tell(vehiclePosition);
            });
        }
    }
}