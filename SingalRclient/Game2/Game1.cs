using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using WebAPIAuthenticationClient;
using Class_Library;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using Class_Library.Base_Classes;


namespace Game2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;


        #region Variables
        string Message;
        string annoucement;
        string type;

        Registration PlayerReg;
        Texture2D textureCollectable;
        Texture2D[] textures;
        string clientID;

        Player player;
        Player Enemy;
        Vector2 startVector = new Vector2(50, 250);
        Player ship_one;
        Projectile newPorjectile;

        bool Game_Started = false;

        float timer;
        int time_Counter;

        Texture2D backgroundTexture;
        Texture2D Player_tex;
        SpriteFont announcement_font;


        KeyboardState oldState, newState;


        enum currentDisplay { Login, Game, Score };
        currentDisplay currentState = currentDisplay.Login;

        enum endGameStatuses { Win, Lose }
        endGameStatuses gameOutcome = endGameStatuses.Win;

        public List<Projectile> Bullets = new List<Projectile>();
        List<Collectable> Collectables = new List<Collectable>();
        List<Collectable> pickUp = new List<Collectable>();
        List<Projectile> destroyBullets = new List<Projectile>();

        static IHubProxy proxy;
        HubConnection connection = new HubConnection("http://localhost:28042/");

        #endregion


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            #region States and proxy
            oldState = Keyboard.GetState();
            graphics.PreferredBackBufferWidth = 800; //set the size of the window
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            proxy = connection.CreateHubProxy("GameHub");
            clientID = connection.ConnectionId;
            #endregion

            #region Declaring actions and proxys and login
            Action<List<Vector2>> ReciveCollectablePositions = reciveCollectablePositions;

            Action<string, string> RecivePlayer = recivePlayerMessage;
            Action<Vector2> ReciveNewPosition = reciveNewPlayerPosition;
            Action<string, Vector2, Vector2> ReciveNewBullet = reciveNewEnemyBullet;
            Action<Vector2> ReciveDiffrentStartposition = reciveDiffrentStartposition;

            proxy.On("otherStartpoint", ReciveDiffrentStartposition);
            proxy.On("sendPlayer", RecivePlayer);
            proxy.On("sendPositionCollectables", ReciveCollectablePositions);
            proxy.On("updatePosition", ReciveNewPosition);
            proxy.On("newBullet", ReciveNewBullet);

            // TODO: Add your initialization logic here
            try
            {
                bool valid = Login.Register(PlayerReg);
                if (valid) Message = ("Player Logged in with Token "); //if (valid) Message = "Player Logged in with Token " + Login.PlayerToken;

                //else Message = Login.PlayerToken;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            base.Initialize();
            #endregion

        }


        protected override void LoadContent()
        {
            #region loading texutres
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTexture = Content.Load<Texture2D>(@"Background\Background");
            Player_tex = Content.Load<Texture2D>(@"Sprites\Player");
            font = Content.Load<SpriteFont>(@"SpriteFont\MessageFont");
            textureCollectable = Content.Load<Texture2D>(@"Sprites\Collectable");
            announcement_font = Content.Load<SpriteFont>(@"SpriteFont\MessageFont");
            #endregion

            #region conneciton message
            Console.WriteLine("Connecting to server");

            connection.Start().Wait();

            Console.WriteLine("Connected to server");
            #endregion

        }
 
        protected override void Update(GameTime gameTime)
        {

            #region Game Logic (movement updating methods etc
            newState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (currentState == currentDisplay.Login)
            {


                player = create_Player(clientID, type);

                if (player != null)
                {
                    proxy.Invoke("SendPlayer");
                }


            }

            if (currentState == currentDisplay.Game)
            {
                if (Game_Started)
                {
                    player.Move(newState);
                    proxy.Invoke("UpdatePosition", player._position);

                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    time_Counter += (int)timer;
                    if (timer >= 1.0F) timer = 0F;



                }

                foreach (var item in Collectables)
                {
                    if (player.CollisiionDetection(item.Rectangle))
                    {
                        pickUp.Add(item);
                        item.IsVisible = false;
                        player.Collect(item);
                    }


                }
                foreach (var item in pickUp)
                {
                    Collectables.Remove(item);
                }

                if (Collectables.Count == 0)
                    currentState = currentDisplay.Score;


                foreach (var item in Collectables)
                {
                    if (player.CollisiionDetection(item.Rectangle))
                    {
                        pickUp.Add(item);
                        item.IsVisible = false;
                        player.Collect(item);
                    }

                    if (Enemy.CollisiionDetection(item.Rectangle))
                    {
                        pickUp.Add(item);
                        item.IsVisible = false;
                        Enemy.Collect(item);
                    }
                }



                if (newState.IsKeyDown(Keys.Space) && oldState != newState && Game_Started)
                {
                    newPorjectile = ship_one.PlayerChar.Shoot(player._position, player.FireDirection);
                    if (newPorjectile != null)
                    {
                        Bullets.Add(newPorjectile);
                        proxy.Invoke("NewBullet", newPorjectile._position, newPorjectile.flyDirection);
                    }

                }

                foreach (var item in Bullets)
                {
                    item.Update();
                    if (OutsideScreen(item))
                    {
                        destroyBullets.Add(item);
                    }
                }


                foreach (var item in pickUp)
                {
                    Collectables.Remove(item);
                }
                foreach (var item in destroyBullets)
                {
                    Bullets.Remove(item);
                }

                pickUp.Clear();
                destroyBullets.Clear();

                if (Collectables.Count == 0)
                    currentState = currentDisplay.Score;
                if (Enemy.PlayerChar.Health <= 0)
                    currentState = currentDisplay.Score;
                if (player.PlayerChar.Health <= 0)
                    currentState = currentDisplay.Score;

                if (currentState == currentDisplay.Score)
                {
                    Game_Started = false;
                    proxy.Invoke("StartGame", Game_Started);
                    if (player.score > Enemy.score)
                        gameOutcome = endGameStatuses.Win;
                    if (player.score < Enemy.score)
                        gameOutcome = endGameStatuses.Lose;

                }
                base.Update(gameTime);


            }
            #endregion

        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            #region timed annoucmenet draw
            if (time_Counter >= 25)
            {
                spriteBatch.DrawString(announcement_font, annoucement, new Vector2(300, 100), Color.Blue);
            }
            #endregion

            #region Game Draw

            if (currentState == currentDisplay.Game) //if game is started
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 800, 600), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f); //draw the background                
                spriteBatch.DrawString(font, "Score: " + Enemy.score.ToString(), new Vector2(700, 0), Color.White);
                spriteBatch.DrawString(font, "Score: " + player.score.ToString(), new Vector2(0, 0), Color.White);
                spriteBatch.End();

                if (Enemy != null)
                    Enemy.Draw(spriteBatch);

                player.Draw(spriteBatch, font); //draw the player

                foreach (var item in Collectables)
                {
                    item.Draw(spriteBatch); // draw the Collectabels at layer 0
                }

                foreach (var item in Bullets)
                {
                    item.Draw(spriteBatch); // draw the Bullets
                }


            }
            #endregion

            #region Login Draw (NOTE ATTEMPT ONLY the console version works fine this is just on the monogame side)
            if (currentState == currentDisplay.Login) //Attempt at login 
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 800, 800), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f); //draw the background
                if (Enemy != null)
                    spriteBatch.DrawString(font, "Login Please", new Vector2(400, 0), Color.White);
                spriteBatch.DrawString(font, "Email " + PlayerReg.Email, new Vector2(200, 200), Color.White);
                spriteBatch.DrawString(font, "Password " + PlayerReg.Password, new Vector2(100, 100), Color.White);
                spriteBatch.End();

            }
            #endregion

            spriteBatch.DrawString(font, Message, new Vector2(10, 10), Color.White);
            spriteBatch.End();

            #region Drawing collectables
            foreach (var item in Collectables)
            {
                item.Draw(spriteBatch); // draw the Collectabels 
            }

            if (currentState == currentDisplay.Score)
            {
                player._position = new Vector2(300, 400);
                Enemy._position = new Vector2(450, 400);

                player.Draw(spriteBatch);
                Enemy.Draw(spriteBatch);
                Vector2 fontPos = new Vector2(player._texture.Width / 2, -10);
                Vector2 namePos = new Vector2(player._texture.Width / 2, player._texture.Height + 10);

                spriteBatch.Begin();
                spriteBatch.DrawString(font, gameOutcome.ToString(), new Vector2(350, 100), Color.BlueViolet, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, player.score.ToString(), fontPos + player._position, Color.White, 0, font.MeasureString(player.score.ToString()) / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, Enemy.score.ToString(), fontPos + Enemy._position, Color.White, 0, font.MeasureString(Enemy.score.ToString()) / 2, 1, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, "Player_1", namePos + player._position, Color.White, 0, font.MeasureString("Player_1") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Player_2", namePos + Enemy._position, Color.White, 0, font.MeasureString("Player_2") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            base.Draw(gameTime);
            #endregion

        }
            #region METHODS
        private void recivePlayerMessage(string arg1, string arg2)
        {
            Enemy = create_Player(arg1,arg2);
            Game_Started = true;

        }
        private void reciveNewPlayerPosition(Vector2 obj)
        {
            Enemy._position = obj;
        }
        private void reciveNewEnemyBullet(string arg1, Vector2 arg2, Vector2 arg3)
        {
            Bullets.Add(new Projectile(arg1, Enemy.PlayerChar._texture, Enemy.PlayerChar.strength, arg2, arg3));
        }
        private Player create_Player(string id, string type)
        {
            Player temp = null;
            if (type != null)
            {

                switch (type.ToUpper()) //check for type and create the character
                {
                    case "FAST":
                        currentState = currentDisplay.Game;
                        temp = new Player(new Character(id, textures[0], 10, 3), startVector, this);
                        break;
                    case "NORMAL":
                        currentState = currentDisplay.Game;
                        temp = new Player(new Character(id, textures[0], 7, 5), startVector, this);
                        break;
                    default:
                        break;
                }
            }
            return temp;
        }

        private void reciveCollectablePositions(List<Vector2> obj)
        {
            foreach (var item in obj)
            {
                Collectables.Add(new Collectable(textureCollectable, item));
            }
        }
        public bool OutsideScreen(Sprite obj)
        {
            if (!obj.Rectangle.Intersects(Window.ClientBounds))
            {
                return true;
            }
            else
                return false;
        }
        private void reciveDiffrentStartposition(Vector2 obj)
        {
            player._position = obj;
        }
        #endregion

    }
}