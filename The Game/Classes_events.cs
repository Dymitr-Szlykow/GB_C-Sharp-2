using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class DeathEventArgs : EventArgs
    {
        public int LastDamage { get; set; }

        public DeathEventArgs(int damage)
        {
            LastDamage = damage;
        }
    }
}
