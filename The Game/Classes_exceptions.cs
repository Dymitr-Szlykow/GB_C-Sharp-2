using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Game
{
    [Serializable]
    public class GameObjectException : Exception
    {
        public GameObjectException() { }
        public GameObjectException(string message) : base(message) { }
        public GameObjectException(string message, Exception innerException) : base(message, innerException) { }
    }
}
