using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI;
using Microsoft.AspNet.SignalR;
using TransitTracker.Hubs;
using TransitTracker.Models;

namespace TransitTracker.Services
{
    public class VehiclePositionController : ApiController
    {
        // GET: api/VehiclePosition
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/VehiclePosition/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/VehiclePosition
        public void Post(Vehicle vehicles)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GpsClientHub>();
            context.Clients.All.NotifyChanged(vehicles);
        }

        // PUT: api/VehiclePosition/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/VehiclePosition/5
        public void Delete(int id)
        {
        }
    }
}
