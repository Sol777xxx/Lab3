using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.ObserverPattern
{
    public struct GameEvent
    {
        public string GameName { get; }
        public string EventMessage { get; }
        public DateTime Time { get; }

        public GameEvent(string gameName, string eventMessage)
        {
            GameName = gameName;
            EventMessage = eventMessage;
            Time = DateTime.Now;
        }
    }
}