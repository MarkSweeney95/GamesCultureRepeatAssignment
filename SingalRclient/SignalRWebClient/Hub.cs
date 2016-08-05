using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Ajax.Utilities;

namespace SignalRWebClient
{
    public class Player

    {
        public string PlayerID;

        public string GamerTag;
        public string UserName;
        public string FirstName;
        public string SecondName;
        public int XP;
        public string clientID;
        public bool playing;

        public static List<Player> players = new List<Player>()

        { new Player { PlayerID = "player1" ,GamerTag="XMARCASX",UserName="deadmpunk",FirstName="m",SecondName="s",XP=100000,clientID="1" },
         new Player { PlayerID = "player2"  }, };

        public Player join(string username)
        {
            if (!playing)
            {
                Player p = players.FirstOrDefault(pl => pl.UserName == username);
                if (p != null && !joined.Contains(p)) // Valid player
                {
                    p.clientID = Context.ConnectionId;
                    joined.Add(p);
                    if (joined.Count() > 1)
                    {
                        Clients.All.play(joined); // Note clients must subscribe to this event
                        playing = true;
                    }
                    return p;
                }
                else return null;
            }
            else return null;
        }

        public void joinUp(string username)
        {
            if (!playing)
            {
                Player p = players.FirstOrDefault(pl => pl.UserName == username);
                if (p != null && !joined.Contains(p)) // Valid player
                {
                    p.clientID = Context.ConnectionId;
                    joined.Add(p);
                    Clients.Caller.joined(p);
                    if (joined.Count() > 1)
                    {
                        Clients.All.play(joined); // Note clients must subscribe to this event
                        playing = true;
                    }
                }
            }


        }




        // Note this 
        public static class HubState

        {

            public static List<Player> players = new List<Player>()

        { new Player { PlayerID = "player1" ,GamerTag="XMARCASX",UserName="deadmpunk",FirstName="m",SecondName="s",XP=100000,clientID="1" },
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
    } }