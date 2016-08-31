using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Ajax.Utilities;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Threading.Tasks;
using Game2;

namespace SignalRWebClient
{
    public class Player

    {
        public enum GAMESTATE { starting, joining, joined, playing, join };

        public string PlayerID;

        public string GamerTag;
        public string UserName;
        public string FirstName;
        public string SecondName;
        public int XP;
        public string clientID;
        public bool playing;
        
        List<Player> joined = new List<Player>();
        List<Player> players = new List<Player>();

        public Player Join(string username)
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
    public class GameHub : Hub
    {

        static Timer t;
        Random r = new Random();
        static List<Vector2> positionsCollectables = new List<Vector2>();
        static List<Vector2> playerOnePositionBarriers = new List<Vector2>();
        static string playerOneChar;
        static string playerOne;
        static string playerTwo;
        static int c = 0;
        static bool gameRunning = false;


        #region StartGame send once

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public void SendPlayer(string charType)
        {
            if (playerOneChar == null)
            {
                SendCollectables();
                playerOneChar = charType;
            }
            else
            {
                SendCollectables();
                Clients.Caller.otherStartpoint(new Vector2(700, 200));
                Clients.Caller.sendPlayer(playerOne, playerOneChar);
                Clients.Others.sendPlayer(Context.ConnectionId, charType);
                gameRunning = true;
                GameStart(gameRunning);
                playerOneChar = null;
            }
        }



        public void SendCollectables()
        {
            if (positionsCollectables.Count == 0 && c == 0)
            {
                c++;
                int temp = r.Next(3, 10);
                for (int i = 0; i < temp; i++) //create collectables
                {
                    positionsCollectables.Add(new Vector2(r.Next(50, 700), r.Next(50, 500)));
                }

            }
            else
            {
                while (c != 1)
                { }
                c = 0;
                Clients.All.sendPositionCollectables(positionsCollectables);

                positionsCollectables.Clear();
            }

        }

        #endregion

        #region Updating Methodes

        public void UpdatePosition(Vector2 newPlayerPositon)
        {
            Clients.Others.updatePosition(newPlayerPositon);
        }

        #endregion

        #region Trigger Methodes

        public void NewBullet(Vector2 startPosition, Vector2 flyDirection)
        {
            Clients.Others.newBullet(Context.ConnectionId, startPosition, flyDirection);
        }


        public void GameStart(bool g)
        {
            gameRunning = g;

        }

        System.Threading.Thread t1 =
            new System.Threading.Thread(t =>
            {
                Game1 game = new Game1();
                game.Run();
            });
        

            System.Threading.Thread t2 =
            new System.Threading.Thread(t =>
            {
                Game1 game1 = new Game1();
                game1.Run();
            });
       


        #endregion
    }

}


