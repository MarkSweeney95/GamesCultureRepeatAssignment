using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Library.Weapons
{
    public class Weapon : Base_Classes.Sprite
    {
        public string createdPlayerID;
        public float damage;
        public float angle;

        public float speed;

        SpriteEffects flip;
        Vector2 moveVector;


        public Weapon(Texture2D _tex, Vector2 _pos) : base(_tex, _pos)
        { }

        public Weapon(string id, Texture2D _tex, float _dam, Vector2 _pos, Vector2 _move, float _ang, float _speed) : base(_tex, _pos)
        {
            createdPlayerID = id;
            damage = _dam;
            angle = _ang;
            moveVector = _move;
            speed = _speed;
        }

        public void WeaponUpdate()
        {
            _position += (moveVector * speed);

            //if (_position.X >= 1280 || _position.Y >= 768)            Destroy object
            //    this = null;
        }

        public override void Draw(SpriteBatch sp)
        {
            base.Draw(sp, angle, flip);
        }

    }
}
