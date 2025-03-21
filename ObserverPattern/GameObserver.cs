using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lab3.ObserverPattern
{
    public class GameObserver : IObserver<GameEvent>
    {
        private IDisposable _unsubscriber;

        public void Subscribe(GameMonitor monitor)
        {
            _unsubscriber = monitor.Subscribe(this);
        }

        public void OnNext(GameEvent value)
        {
            Console.WriteLine($"[Сповіщення] {value.GameName}: {value.EventMessage} ({value.Time})");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[Помилка сповіщення] {error.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("[Сповіщення] Гра завершилася.");
        }

        public void Unsubscribe()
        {
            _unsubscriber?.Dispose();
        }
    }
}
