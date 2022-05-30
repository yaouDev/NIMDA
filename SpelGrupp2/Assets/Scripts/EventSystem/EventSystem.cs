using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem {
    public class EventSystem : MonoBehaviour {
        Dictionary<System.Type, HashSet<EventListener>> eventListeners;
        /* ugly solution with too much redundancy. But since the "wrapper" (likely because of its member "Method")
        in UnregisterListener() doesn't work with HashSet.Contains this is needed
        to keep track of which listeneres have been added so that they can be removed. 
        The inner dictionary is for if a target has more than one listener*/
        Dictionary<System.Object, Dictionary<System.Object, EventListener>> listenersByTarget;
        void OnEnable() {
            current = this;
            eventListeners = new Dictionary<System.Type, HashSet<EventListener>>();
            listenersByTarget = new Dictionary<System.Object, Dictionary<System.Object, EventListener>>();
        }

        static private EventSystem current;
        public static EventSystem Current {
            get {
                if (current == null) {
                    current = GameObject.FindObjectOfType<EventSystem>();
                }
                return current;
            }
        }

        public delegate void EventListener(Event eventToListenFor);
        public void RegisterListener<T>(System.Action<T> listener) where T : Event {
            System.Type eventType = typeof(T);
            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null) eventListeners[eventType] = new HashSet<EventListener>();
            EventListener wrapper = (eventToListenFor) => { listener((T)eventToListenFor); };
            eventListeners[eventType].Add(wrapper);
            if (!listenersByTarget.ContainsKey(listener.Target)) listenersByTarget.Add(listener.Target, new Dictionary<System.Object, EventListener>());
            listenersByTarget[listener.Target].Add(listener, wrapper);
            eventType = eventType.BaseType;

        }

        public void UnregisterListener<T>(System.Action<T> listener) where T : Event {
            System.Type eventType = typeof(T);
            if (eventListeners.ContainsKey(eventType) && eventListeners[eventType] != null && listenersByTarget.ContainsKey(listener.Target)) {
                eventListeners[eventType].Remove(listenersByTarget[listener.Target][listener]);
                if (listenersByTarget[listener.Target].Count == 0) listenersByTarget.Remove(listener.Target);
                else listenersByTarget[listener.Target].Remove(listener);
            }
        }

        public void FireEvent(Event eventToFire) {
            System.Type eventType = eventToFire.GetType();
            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null) return;
            /* walks up the event hierarchy and makes sure that listeners to the superclass of the event also get called */
            do {
                foreach (EventListener eventListener in eventListeners[eventType]) {
                    eventListener(eventToFire);
                }
                eventType = eventType.BaseType;
            } while (eventType != typeof(Event));


        }
    }
}


