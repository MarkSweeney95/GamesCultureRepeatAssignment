
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Class_Library.Base_Classes
{
    public class Player : Sprite
    {
        #region Varlibles
        static int ID = 1;
        int PlayerID;
        public Character PlayerChar;
        //public Vector2 Position;
        public Vector2 FireDirection = new Vector2(1, 0);
        
        public int score;
        Game game;
        #endregion

        #region setting up player
        public Player(Character character, Vector2 pos, Game g) : base(character._texture, pos) //create the player
        {
            PlayerID = ID++;
            PlayerChar = character;
            //Position = pos;
           
            game = g;
        }
        #endregion

        #region Move and Shoot
        public void Move(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.W) && _position.Y > 0) //check the move direction and update position
            {
                _position -= new Vector2(0, PlayerChar.movementSpeed);
                FireDirection = new Vector2(0, -1);
            }
            if (state.IsKeyDown(Keys.S) && _position.Y < 550)
            {
                _position += new Vector2(0, PlayerChar.movementSpeed);
                FireDirection = new Vector2(0, 1);
            }
            if (state.IsKeyDown(Keys.D) && _position.X < 760)
            {
                _position += new Vector2(PlayerChar.movementSpeed, 0);
                FireDirection = new Vector2(1, 0);
            }
            if (state.IsKeyDown(Keys.A) && _position.X > 0)
            {
                _position -= new Vector2(PlayerChar.movementSpeed, 0);
                FireDirection = new Vector2(-1, 0);
            }

        }
        #endregion

        #region Collecting collectables
        public void Collect(Collectable c)
        {
            score += c.Value;
        }
        #endregion

        #region draw method
        public void Draw(SpriteBatch sp, SpriteFont sf)
        {
            sp.Begin();
            sp.Draw(PlayerChar._texture, _position, pColor); 
            sp.End();
        }
        #endregion

    }
}
