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

    class ExpiringSub<TEventArgs, TReactionParams> where TEventArgs : EventArgs  // TODO, не проверял
    {
        int countdown;
        EventHandler<TEventArgs> _manager;
        Action<TReactionParams> _reAction;
        TReactionParams _parameters;
        public bool expired;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term">на какой срок (на сколько вызовов/циклов)</param>
        /// <param name="cycleManager">на какое событие</param>
        /// <param name="reaction">какое действие</param>
        /// <param name="parameters"></param>
        public ExpiringSub(int term, EventHandler<TEventArgs> cycleManager, Action<TReactionParams> reaction, TReactionParams parameters)
        {
            countdown = term;
            _manager = cycleManager;
            _reAction = reaction;
            _parameters = parameters;
            cycleManager += Reaction;
            expired = false;
        }

        public void Reaction (object sender, TEventArgs e)
        {
            if (--countdown > 0)
            {
                _reAction.Invoke(_parameters);
            }
            else
            {
                _manager -= Reaction;
                expired = true;
            }
        }

        public void Unsubscribe()
        {
            _manager -= Reaction;
            expired = true;
        }

        public static void KeepClean(List<ExpiringSub<TEventArgs, TReactionParams>> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].expired) list.RemoveAt(i);
            }
        }
    }
}
