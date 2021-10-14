using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    class EndGameEventArgs : EventArgs  // TODO уточнить, доработать
    {
        public string _onScreenMessage;
        public string _logMessage;

        public EndGameEventArgs()
        {
            _onScreenMessage = string.Empty;
            _logMessage = string.Empty;
        }
        public EndGameEventArgs(string onScreenMessage)
        {
            _onScreenMessage = onScreenMessage;
            _logMessage = string.Empty;
        }
        public EndGameEventArgs(string onScreenMessage, string logMessage)
        {
            _onScreenMessage = onScreenMessage;
            _logMessage = logMessage;
        }
    }

    class DeathEventArgs : EndGameEventArgs
    {
        public int lastDamage;
        public DeathEventArgs(int damage, string onScreenMessage) : base(onScreenMessage) => lastDamage = damage;
        public DeathEventArgs(int damage, string onScreenMessage, string logMessage) : base(onScreenMessage, logMessage) => lastDamage = damage;
    }
}
