using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CallbackSystem
{
    public class EventSystem_Inactive : MonoBehaviour
    {
        delegate void EventListener(EventInfo_Inactive ei);
        Dictionary<System.Type, List<EventListener>> eventListeners;

        static private EventSystem_Inactive _Current;

        void OnEnable()
        {
            _Current = this;
        }

        static public EventSystem_Inactive Current
        {
            get
            {
                if(_Current == null)
                {
                    _Current = GameObject.FindObjectOfType<EventSystem_Inactive>();

                }
                return _Current;
            }
        }

        public void RegisterListener<T>(System.Action<T> listener) where T : EventInfo_Inactive
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

        public void UnregisterListener<T>(System.Action<T> listener) where T : EventInfo_Inactive
        {
            //System.Type etype = typeof(T);
            //if(eventListeners != null && (eventListeners.ContainsKey(etype) || eventListeners [etype] != null))
            //{
            //    eventListeners.Remove(etype);
            //}
        }

        public void FireEvent(EventInfo_Inactive ei)
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

