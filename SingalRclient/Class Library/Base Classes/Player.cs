using Class_Library.Ship;
using Class_Library.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Class_Library.Base_Classes
{
    public class Player : Sprite
    {
        public string UserName; //username which appears in the chat
        public Base_Ship Ship;
        public Weapon weapon;
        public bool isActive;
        public int score;
        public Vector2 FireDirection = new Vector2(1, 0);

        public Player(string uName, Base_Ship _ship, Weapon _weapon)
        {
            UserName = uName;
            Ship = _ship;
            weapon = _weapon;
            Ship.weapon = weapon;
            isActive = true;
        }

        public void Collect(Collectable c)
        {
            score += c.Value;
        }
        public void Move(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.W) && _position.Y > 0) 
            {
                _position -= new Vector2(0, Ship.movementSpeed);
                FireDirection = new Vector2(0, -1);
            }
            if (state.IsKeyDown(Keys.S) && _position.Y < 550)
            {
                _position += new Vector2(0, Ship.movementSpeed);
                FireDirection = new Vector2(0, 1);
            }
            if (state.IsKeyDown(Keys.D) && _position.X < 760)
            {
                _position += new Vector2(Ship.movementSpeed, 0);
                FireDirection = new Vector2(1, 0);
            }
            if (state.IsKeyDown(Keys.A) && _position.X > 0)
            {
                _position -= new Vector2(Ship.movementSpeed, 0);
                FireDirection = new Vector2(-1, 0);
            }
        }

    }
}
