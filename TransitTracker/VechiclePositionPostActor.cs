using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Akka.Actor;

namespace TransitTracker
{
    internal class VechiclePositionPostActor : TypedActor, IHandle<VehiclePositionUpdated>
    {
        public void Handle(VehiclePositionUpdated message)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:60406");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var responseTask = client.PostAsJsonAsync("Api/VehiclePosition", message).Result;
                    if (!responseTask.IsSuccessStatusCode)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}