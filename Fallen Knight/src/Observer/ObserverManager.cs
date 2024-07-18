using Fallen_Knight.GameAssets.Interface;
using System.Collections.Generic;

namespace Fallen_Knight.GameAssets.Observer
{
    public static class ObserverManager
    {
        private static List<IObserver> observers = new List<IObserver>();

        public static void CameraEventSubscribe(IObserver subscriber)
        {
            observers.Add(subscriber);
        }
        public static void Unsubscribe(IObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        public static void NotifyCamera()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }
}
