using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRWebClient
{
    public class Player

    {
        public string PlayerID;
        // Add more fields here
    }


    // Note this 
    public static class HubState

    {
        public static List<Player> players = new List<Player>()

        { new Player { PlayerID = "player1"  },
         new Player { PlayerID = "player2"  },
         // etc
        };

    }


    public class MyHub1 : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void sendPlayers()
        {
            Clients.Caller.RecievePlayers(HubState.players);
        }

        public List<Player> getPlayers()
        {
            return HubState.players;
        }
    }
}