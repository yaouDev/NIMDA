using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Callbacks
{
    public class EventSystem : MonoBehaviour
    {
        delegate void EventListener(EventInfo ei);
        Dictionary<System.Type, List<EventListener>> eventListeners;

        static private EventSystem _Current;

        void OnEnable()
        {
            _Current = this;
        }

        static public EventSystem Current
        {
            get
            {
                if(_Current == null)
                {
                    _Current = GameObject.FindObjectOfType<EventSystem>();

                }
                return _Current;
            }
        }

        public void RegisterListener<T>(System.Action<T> listener) where T : EventInfo
        {
            System.Type etype = typeof(T);
            if(eventListeners == null)
            {
                eventListeners = new Dictionary<System.Type, List<EventListener>>();
            }

            if(eventListeners.ContainsKey(etype) == false || eventListeners[etype] == null)
            {
                eventListeners[etype] = new List<EventListener>();
            }
            EventListener wrapper = (ei) => { listener((T)ei); };
            eventListeners[etype].Add(wrapper);
        }

        public void UnregisterListener<T>(System.Action<T> listener) where T : EventInfo
        {
            
        }

        public void FireEvent(EventInfo ei)
        {
            System.Type trueEventInfoClass = ei.GetType();
            if(eventListeners == null || eventListeners[trueEventInfoClass] == null){
                return;
            }
            foreach(EventListener el in eventListeners[trueEventInfoClass])
            {
                el(ei);
            }
        }
    }
}

