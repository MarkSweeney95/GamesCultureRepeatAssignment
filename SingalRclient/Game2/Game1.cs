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
        Registration PlayerReg;
        Texture2D textureCollectable;
        string clientID;
        Player player;
        Vector2 startVector = new Vector2(50, 250);

        Texture2D backgroundTexture;
        Texture2D[] textures;
    
   
      

        enum currentDisplay { Selection, Game, Score };
        currentDisplay currentState = currentDisplay.Selection;

        enum endGameStatuses { Win, Lose }
        endGameStatuses gameOutcome = endGameStatuses.Win;

        List<Collectable> Collectables = new List<Collectable>();
        List<Collectable> pickUp = new List<Collectable>();

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
            graphics.PreferredBackBufferWidth = 800; //set the size of the window
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            
            proxy = connection.CreateHubProxy("GameHub");
            clientID = connection.ConnectionId;


            Action<List<Vector2>> ReciveCollectablePositions = reciveCollectablePositions;


            proxy.On("sendPositionCollectables", ReciveCollectablePositions);

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
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>(@"SpriteFont\MessgaeFont");
            textureCollectable = Content.Load<Texture2D>("Collectable");
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            foreach (var item in Collectables)
            {
                if (player.CollisiionDetection(item.Rectangle))
                {
                    pickUp.Add(item);
                    item.IsVisible = false;
                    player.Collect(item);
                }

                //if (Enemy.CollisiionDetection(item.Rectangle))
                //{
                //    pickUp.Add(item);
                //    item.IsVisible = false;
                //    Enemy.Collect(item);
                //}
            }

            foreach (var item in pickUp)
            {
                Collectables.Remove(item);
            }

            if (Collectables.Count == 0)
                currentState = currentDisplay.Score;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, Message, new Vector2(10, 10), Color.White);
            spriteBatch.End();
            // TODO: Add your drawing code here
            foreach (var item in Collectables)
            {
                item.Draw(spriteBatch); // draw the Collectabels at layer 0
            }
            base.Draw(gameTime);

        }


        private void reciveCollectablePositions(List<Vector2> obj)
        {
            foreach (var item in obj)
            {
                Collectables.Add(new Collectable(textureCollectable, item));
            }
        }

    }
    }
