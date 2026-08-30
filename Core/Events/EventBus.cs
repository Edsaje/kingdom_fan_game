using System;
using System.Collections.Generic;

namespace KingdomCore.Events
{
    public interface IGameEvent { }

    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> callback) where T : IGameEvent
        {
            Type eventType = typeof(T);
            
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }
            
            _subscribers[eventType].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IGameEvent
        {
            Type eventType = typeof(T);
            
            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(callback);
            }
        }

        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            Type eventType = typeof(T);
            
            if (_subscribers.ContainsKey(eventType))
            {
                var handlers = new List<Delegate>(_subscribers[eventType]);
                
                foreach (Delegate handler in handlers)
                {
                    if (handler is Action<T> action)
                    {
                        action.Invoke(gameEvent);
                    }
                }
            }
        }
    }
}
