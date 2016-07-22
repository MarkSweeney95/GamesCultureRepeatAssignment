using Class_Library.Ship;
using Class_Library.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Class_Library.Base_Classes
{
    public class Player : Sprite
    {
        public string UserName; //username which appears in the chat
        public Base_Ship Ship;
        public Weapon weapon;
        public bool isActive;

        public Player(string uName, Base_Ship _ship, Weapon _weapon)
        {
            UserName = uName;
            Ship = _ship;
            Weapon = _weapon;
            Ship.weapon = Weapon;
            isActive = true;
        }

    }
}
