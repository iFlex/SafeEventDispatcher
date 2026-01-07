using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Events
{
    public class SafeEventDispatcher<T>
    {
        List<Action<T>> listenerQueues = new List<Action<T>>();
        private bool lockListenerList = false;
        private List<Action<T>> deferedAddQ = new List<Action<T>>();
        private List<Action<T>> deferedRemoveQ = new List<Action<T>>();

        public int Count()
        {
            return listenerQueues.Count;
        }
        
        public void Dispatch(T evt)
        {
            lockListenerList = true;
            for(int i = 0; i < listenerQueues.Count; i++)
            {
                try
                {
                    listenerQueues[i].Invoke(evt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SafeEventDispatch]({this}) handle:{listenerQueues[i]}");
                    Debug.LogException(e);
                }
            }
            lockListenerList = false;
            ProcessDeferredOps();
        }

        void ProcessDeferredOps()
        {
            if (deferedRemoveQ.Count > 0)
            {
                foreach (Action<T> deferred in deferedRemoveQ)
                {
                    listenerQueues.Remove(deferred);
                }
                deferedRemoveQ.Clear();
            }
            if (deferedAddQ.Count > 0)
            {
                foreach (Action<T> deferred in deferedAddQ)
                {
                    listenerQueues.Add(deferred);
                }
                deferedAddQ.Clear();
            }
        }
        
        public void AddEventListener(Action<T> listener)
        {
            if (lockListenerList)
            {
                deferedAddQ.Add(listener);
            }
            else
            {
                listenerQueues.Add(listener);   
            }
        }
        
        public void RemoveEventListener(Action<T> listener)
        {
            if (lockListenerList)
            {
                deferedRemoveQ.Add(listener);
            }
            else
            {
                listenerQueues.Remove(listener);   
            }
        }

        public void Clear()
        {
            listenerQueues.Clear();
            deferedAddQ.Clear();
            deferedRemoveQ.Clear();
        }
    }
}