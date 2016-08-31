using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Library.Base_Classes
{
    public class Projectile : Sprite
    {

        public string createdPlayerID;
    
        private float speed = 10f;
        public int damage;
        public Vector2 flyDirection;


        public Projectile(string id, Texture2D tex, int st, Vector2 pos, Vector2 direction) : base(tex, pos)
        {
            createdPlayerID = id;
            damage = st;
            flyDirection = new Vector2(direction.X, direction.Y);
          
        }

        public void Update()
        {
            _position = _position + (flyDirection * speed);
        }

        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width / 2, _texture.Height / 2);
            }

            set
            {
                base.Rectangle = value;
            }
        }

        public override void Draw(SpriteBatch sp)
        {
            if (_texture != null)
            {
                sp.Begin();
                sp.Draw(_texture, _position, null, pColor, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
                sp.End();

            }
        }

        float weight;

        public float Weight
        {
            get
            {
                return weight;
            }

            set
            {
                weight = value;
            }
        }

        public Projectile(float _weight, Texture2D _tex, Vector2 _pos) : base(_tex, _pos)
        {
            Weight = _weight;
        }

    }
}

