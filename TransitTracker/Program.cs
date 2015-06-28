using System.Data.Entity;
using Akka.Actor;
using TransitTracker.Data;

namespace TransitTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TransitTrackerContext>());

            using (var system = ActorSystem.Create("MyAkkaSystem"))
            {
                var feedURL = "https://gtfsrt.api.translink.com.au/feed";

                var databaseActor = system.ActorOf<DatabaseActor>();

                var feedDownloaderActorProps = Props.Create<FeedDownloaderActor>(databaseActor);
                var feedDownloaderActor = system.ActorOf(feedDownloaderActorProps, "FeedDownloaderActor");

                feedDownloaderActor.Tell(new DownloadFeed {URL = feedURL});

                system.AwaitTermination();
            }
        }
    }
}