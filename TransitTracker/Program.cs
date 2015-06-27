using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using ProtoBuf;
using transit_realtime;

namespace TransitTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("MyAkkaSystem"))
            {
                var feedURL = "https://gtfsrt.api.translink.com.au/feed";
                var feedDownloaderActor = system.ActorOf<FeedDownloaderActor>();
                feedDownloaderActor.Tell(new DownloadFeed {URL = feedURL});

                system.AwaitTermination();
            }
        }

       

        public class FeedDownloaderActor : TypedActor, IHandle<DownloadFeed>
        {
            private IActorRef _router = null;
            private Dictionary<string, IActorRef> createdActors = new Dictionary<string, IActorRef>();

            public void Handle(DownloadFeed message)
            {
                {
                    while (true)
                    {
                        var req = WebRequest.Create(message.URL);
                        var feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());

                        foreach (var entity in feed.entity.Where(a => !a.is_deleted && a.vehicle != null))
                            //.Where(b => b.vehicle.vehicle.label == "1006"))
                        {
                            //Console.WriteLine(string.Join("\t", message.vehicle.id, message.vehicle.label, message.current_status, message.position.latitude + ", " + message.position.longitude, UnixTimeStampToDateTime(message.timestamp).TimeOfDay));

                            var vehicleId = entity.vehicle.vehicle.id;
                            if (!createdActors.ContainsKey(vehicleId))                            
                            {
                                var actor = Context.ActorOf(Props.Create<VehicleActor>());
                                createdActors.Add(vehicleId, actor);
                                actor.Tell(entity.vehicle);
                            }
                            else
                            {
                                _router.Tell(entity.vehicle);
                            }
                        }

                        if (_router == null)
                        {
                            _router = Context.ActorOf(Props.Empty.WithRouter(new ConsistentHashingGroup(createdActors.Values)
                                .WithHashMapping(a => (a as VehiclePosition)?.vehicle.id)));
                        }

                        
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        Console.WriteLine("Complete...");
                        Console.WriteLine();
                        Thread.Sleep(TimeSpan.FromSeconds(4));
                        //Console.ReadLine();
                    }

                };
            }
        }
    }

    public class DownloadFeed
    {
        public string URL { get; set; }
    }

    public class VehicleActor : TypedActor, IHandle<VehiclePosition>
    {
        private string vehicleId;
        private float lastLat;
        private float lastLong;

        public void Handle(VehiclePosition message)
        {
            vehicleId = message.vehicle.id;

            if (vehicleId != null
                && ((lastLat != 0 && lastLat != message.position.latitude)
                    || (lastLong != 0 && lastLong != message.position.longitude)))
            {
                //Debugger.Break();
                Console.WriteLine(string.Join("\t", message.vehicle.id, message.vehicle.label, message.current_status, message.position.latitude + ", " + message.position.longitude, UnixTimeStampToDateTime(message.timestamp).TimeOfDay));
            }

            lastLat = message.position.latitude;
            lastLong = message.position.longitude;

            //Console.WriteLine(string.Join("\t", message.vehicle.id, message.vehicle.label, message.current_status, message.position.latitude + ", " + message.position.longitude, UnixTimeStampToDateTime(message.timestamp).TimeOfDay));
            //Console.WriteLine(string.Join("\t", message.vehicle.id, message.vehicle.label, message.current_status, message.position.latitude + ", " + message.position.longitude, UnixTimeStampToDateTime(message.timestamp).TimeOfDay));
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
