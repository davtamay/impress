using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Komodo.Utilities;
using Komodo.Runtime;

//namespace Komodo.Runtime
//{
    [RequireComponent(typeof(MainUIReferences))]
    public class KomodoEventManager : SingletonComponent<KomodoEventManager>
{
    public static KomodoEventManager Instance
    {
        get { return ((KomodoEventManager)_Instance); }
        set { _Instance = value; }
    }
    //public void Start()
    //    {
    //    }

        // From Unity Learn Tutorial: 
        // Create a Simple Messaging System with Events 
        // Tutorial Last Updated June 3rd, 2019
        // "As an introduction to UnityActions and UnityEvents, 
        // we will create a simple messaging system which will 
        // allow items in our projects to subscribe to events, 
        // and have events trigger actions in our games. This 
        // will reduce dependencies and allow easier maintenance 
        // of our projects."

        /* This dictionary will help hold references (String) to events and events (UnityEvent) themselves */
        //private Dictionary <string, UnityEvent> eventDictionary;


    //    private static KomodoEventManager eventManager;

    //public static KomodoEventManager Instance
    //{
    //    get
    //    {
    //        if (!eventManager)
    //        {
    //            eventManager = FindObjectOfType(typeof (KomodoEventManager)) as KomodoEventManager;

    //            if (!eventManager)
    //            {
    //                Debug.LogError("There needs to be one active EventManager script in your scene.");
    //            }
    //            else
    //            {
    //                eventManager.Init();
    //            }
    //        }

    //        return eventManager;
    //    }
    //}

    Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
        /* a method to initialize the eventManager */
        //void Init ()
        //{
        //    if (eventDictionary == null)
        //    {
               
        //    }
        //}

        /* This method first checks the dictionary and see if the dictionary has a key that pairs to 
        whatever we want to add. If there is a key, we add to it. If not, we create a new Unity event
        and we add the listener to it and push it to the dictionary.  */
        public void StartListening (string eventName, UnityAction listener)
        {
            if (!Instance)
            {
                Debug.LogError("Tried to StartListening but KomodoEventManager Instance was not found.");

                return;
            }

            if (Instance.eventDictionary == null)
            {
                Debug.LogError("Tried to StartListening but KomodoEventManager Instance had no eventDictionary.");

                return;
            }

            if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent existingEvent))
            {
                existingEvent.AddListener(listener);
            }
            else
            { 
                UnityEvent newEvent = new UnityEvent();

                newEvent.AddListener(listener);

                Instance.eventDictionary.Add(eventName, newEvent);
            }
        }

        /* This method will stop eventManager from listening*/
        public  void StopListening (string eventName, UnityAction listener)
        {
            if (Instance == null)
            {
                return;
            }

            if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent existingEvent))
            {
                existingEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent (string eventName)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent existingEvent))
            {
                existingEvent.Invoke();
            }
        }
    }
//}
