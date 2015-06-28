using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace TransitTracker.Hubs
{
    public class GpsClientHub : Hub
    {
        private List<string> clients = new List<string>();

        public void NotifyChanged()
        {
            Clients.All.notifyChanged();
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Notify(string connectionId)
        {
            clients.Add(connectionId);
        }
    }
}