using System;
using System.Collections.Generic;
using UnityEngine;

namespace MutedMelody.Core
{
    /// <summary>
    /// A simple marker interface. Any struct or class that wants to be 
    /// sent through the EventBus must implement this.
    /// </summary>
    public interface IGameEvent { }

    /// <summary>
    /// The central nervous system of the game. Allows decoupled communication 
    /// between different systems via Publish/Subscribe.
    /// </summary>
    public static class EventBus
    {
        // The core requirement: A dictionary mapping Event Types to Delegates
        private static readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Registers a method to listen for a specific event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> onEvent) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (_events.ContainsKey(eventType))
            {
                // If there are already listeners, append this new one to the delegate chain
                _events[eventType] = Delegate.Combine(_events[eventType], onEvent);
            }
            else
            {
                // First listener for this event type
                _events.Add(eventType, onEvent);
            }
        }

        /// <summary>
        /// Unregisters a method from listening to a specific event type.
        /// Critical: Always call this in OnDisable or OnDestroy to prevent memory leaks!
        /// </summary>
        public static void Unsubscribe<T>(Action<T> onEvent) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (_events.ContainsKey(eventType))
            {
                Delegate currentDelegate = _events[eventType];
                currentDelegate = Delegate.Remove(currentDelegate, onEvent);

                if (currentDelegate == null)
                {
                    // No one is listening anymore, clean up the dictionary
                    _events.Remove(eventType);
                }
                else
                {
                    _events[eventType] = currentDelegate;
                }
            }
        }

        /// <summary>
        /// Broadcasts an event to all subscribed listeners.
        /// </summary>
        public static void Publish<T>(T gameEvent) where T : IGameEvent
        {
            Type eventType = typeof(T);

            if (_events.TryGetValue(eventType, out Delegate existingDelegate))
            {
                // Cast the delegate back to the specific Action type and invoke
                Action<T> callback = existingDelegate as Action<T>;
                callback?.Invoke(gameEvent);
            }
        }

        /// <summary>
        /// Clears all event subscriptions. Useful when transitioning between major game states
        /// or doing a hard reset of the scene to prevent dangling references.
        /// </summary>
        public static void ClearAll()
        {
            _events.Clear();
        }
    }
}