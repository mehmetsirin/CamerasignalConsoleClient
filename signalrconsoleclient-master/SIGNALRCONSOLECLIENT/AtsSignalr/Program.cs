using Microsoft.AspNet.SignalR.Client;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsSignalr
{
    internal class Program
    {

        static HubConnection hubConnection = null;
        static IHubProxy myHub = null;
         
        private static void Main(string[] args)
        {
            //Set connection

            //string url = "http://localhost:53306//signalr";
            var url = "http://ats2.rota.net.tr/signalr";
            string code = "257";

            hubConnection = new HubConnection(url, "key="+code);

            //To enable client-side logging, set the TraceLevel and TraceWriter properties on the connection object.
            //Logging client events to a text file under SIGNALRCONSOLECLIENT._Console\bin\Debug
            var writer = new StreamWriter("clientLog.txt");
            writer.AutoFlush = true;
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = writer;

            //Make proxy to hub based on hub name on server
            myHub = hubConnection.CreateHubProxy("RealDataNew");

            //Start connection
            hubConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)

                    Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                else
                    Console.WriteLine("Connected:" + hubConnection.ConnectionId);
            }).Wait();

            Console.WriteLine(hubConnection.ConnectionId);


            myHub.On<dynamic>("updateLocation", param =>
            {
                Console.WriteLine(param);
            });


            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();

            #region Handle Connection Lifetime Events
            //Raised when the client detects a slow or frequently dropping connection.
            hubConnection.ConnectionSlow += () => Console.WriteLine("Connection slow");
            //Raised when the connection has disconnected.
            hubConnection.Closed += () => Console.WriteLine("Connection stopped");
            //Raised when the underlying transport has reconnected.
            hubConnection.Reconnected += () => Console.WriteLine("Reconnected");
            //Raised when the underlying transport begins reconnecting.
            hubConnection.Reconnecting += () => Console.WriteLine("Reconnecting");
            //Raised when the connection state changes. Provides the old state and the new state.
            //ConnectionState Enumeration: Connecting, Connected, Reconnecting, Disconnected
            hubConnection.StateChanged += (change) => Console.WriteLine("ConnectionState changed from: " + change.OldState + " to " + change.NewState);
            //Raised when any data is received on the connection. Provides the received data.
            //hubConnection.Received += (data) => Console.WriteLine("Connection received" + data);
            #endregion Handle Connection Lifetime Events

            //To handle errors that SignalR raises, you can add a handler for the Error event on the connection object.
            hubConnection.Error += ex => Console.WriteLine("SignalR error: {0}", ex.Message);

            Console.Read();
            hubConnection.Stop();
        }
    }
}
