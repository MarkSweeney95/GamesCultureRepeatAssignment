using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    // Note this class has to be the same on the Hub as the client. See the Hub Code.
    public class Player

    {
        public string PlayerID;
        // Add more fields here
    }

    class Program
    {
        static IHubProxy proxy;
        static void Main(string[] args)
        {

            HubConnection connection = new HubConnection("http://localhost:28042/");
            proxy = connection.CreateHubProxy("MyHub1");
            connection.Start().Wait();
            connection.Received += Connection_Received;
            proxy.Invoke<List<Player>>("getPlayers").ContinueWith((callback) =>
            {
                foreach (Player p in callback.Result)
                {
                    Console.WriteLine("Player ID is {0} ", p.PlayerID);
                }
            }).Wait();

            Console.WriteLine("I'm back in the main thread. Press return to end");
            Console.ReadLine();
        }

        private static void Connection_Received(string obj)
        {
            Console.WriteLine("Message recieved to {0} ", obj);
        }
    }
}
