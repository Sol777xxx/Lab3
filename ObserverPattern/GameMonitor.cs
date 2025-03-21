using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.ObserverPattern
{
    public class GameMonitor : IObservable<GameEvent>
    {
        private List<IObserver<GameEvent>> observers = new List<IObserver<GameEvent>>();

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<GameEvent>> _observers;
            private IObserver<GameEvent> _observer;

            public Unsubscriber(List<IObserver<GameEvent>> observers, IObserver<GameEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<GameEvent> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        public void Notify(GameEvent gameEvent)
        {
            foreach (var observer in observers)
                observer.OnNext(gameEvent);
        }

        public void Complete()
        {
            foreach (var observer in observers)
                observer.OnCompleted();
            observers.Clear();
        }
    }
}
