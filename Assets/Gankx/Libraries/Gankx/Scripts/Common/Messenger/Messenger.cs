//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define REQUIRE_LISTENER

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    internal static class Messenger
    {
        #region Internal variables

        //Disable the unused variable warning
#pragma warning disable 0414
        //Ensures that the MessengerHelper will be created automatically upon start of the game.
        // static private MessengerHelper messengerHelper = ( new GameObject("MessengerHelper") ).AddComponent<MessengerHelper>();
#pragma warning restore 0414

        public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        //Message handlers that should never be removed, regardless of calling Cleanup
        public static List<string> permanentMessages = new List<string>();

        #endregion

        #region Helper methods

        //Marks a certain message as permanent.
        public static void MarkAsPermanent(string eventType)
        {
#if LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
#endif

            permanentMessages.Add(eventType);
        }


        public static void Cleanup()
        {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
#endif

            var messagesToRemove = new List<string>();

            foreach (var pair in eventTable)
            {
                var wasFound = false;

                foreach (var message in permanentMessages)
                {
                    if (pair.Key == message)
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                {
                    messagesToRemove.Add(pair.Key);
                }
            }

            foreach (var message in messagesToRemove)
            {
                eventTable.Remove(message);
            }
        }

        public static void PrintEventTable()
        {
            Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");

            foreach (var pair in eventTable)
            {
                Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
            }

            Debug.Log("\n");
        }

        #endregion

        #region Message logging and exception throwing

        public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif

            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            var d = eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format(
                    "Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}",
                    eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif

            if (eventTable.ContainsKey(eventType))
            {
                var d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format(
                        "Attempting to remove listener with for event type \"{0}\" but current listener is null.",
                        eventType));
                }

                if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format(
                        "Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}",
                        eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
        }

        public static void OnListenerRemoved(string eventType)
        {
            if (eventTable.ContainsKey(eventType) && eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        public static void OnBroadcasting(string eventType)
        {
#if REQUIRE_LISTENER
        if (!eventTable.ContainsKey(eventType)) {
            throw new BroadcastException(string.Format("Broadcasting message \"{0}\" but no listener found. Try marking the message with Messenger.MarkAsPermanent.", eventType));
        }
#endif
        }

        public static BroadcastException CreateBroadcastSignatureException(string eventType)
        {
            return new BroadcastException(string.Format(
                "Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.",
                eventType));
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }

        #endregion

        #region AddListener

        //No parameters
        public static void AddListener(string eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback) eventTable[eventType] + handler;
        }

        public static void AddListenerPermanent(string eventType, Callback handler)
        {
            AddListener(eventType, handler);
            MarkAsPermanent(eventType);
        }

        //Single parameter
        public static void AddListener<T>(string eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T>) eventTable[eventType] + handler;
        }

        public static void AddListenerPermanent<T>(string eventType, Callback<T> handler)
        {
            AddListener(eventType, handler);
            MarkAsPermanent(eventType);
        }

        //Two parameters
        public static void AddListener<T, U>(string eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U>) eventTable[eventType] + handler;
        }

        public static void AddListenerPermanent<T, U>(string eventType, Callback<T, U> handler)
        {
            AddListener(eventType, handler);
            MarkAsPermanent(eventType);
        }

        //Three parameters
        public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Callback<T, U, V>) eventTable[eventType] + handler;
        }

        public static void AddListenerPermanent<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            AddListener(eventType, handler);
            MarkAsPermanent(eventType);
        }

        #endregion

        #region RemoveListener

        //No parameters
        public static void RemoveListener(string eventType, Callback handler)
        {
            OnListenerRemoving(eventType, handler);
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Callback) eventTable[eventType] - handler;
            }

            OnListenerRemoved(eventType);
        }

        //Single parameter
        public static void RemoveListener<T>(string eventType, Callback<T> handler)
        {
            OnListenerRemoving(eventType, handler);
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Callback<T>) eventTable[eventType] - handler;
            }

            OnListenerRemoved(eventType);
        }

        //Two parameters
        public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
        {
            OnListenerRemoving(eventType, handler);
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Callback<T, U>) eventTable[eventType] - handler;
            }

            OnListenerRemoved(eventType);
        }

        //Three parameters
        public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
        {
            OnListenerRemoving(eventType, handler);
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = (Callback<T, U, V>) eventTable[eventType] - handler;
            }

            OnListenerRemoved(eventType);
        }

        #endregion

        #region Broadcast

        //No parameters
        public static void Broadcast(string eventType)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                var callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Single parameter
        public static void Broadcast<T>(string eventType, T arg1)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                var callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Two parameters
        public static void Broadcast<T, U>(string eventType, T arg1, U arg2)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                var callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Three parameters
        public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                var callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        #endregion
    }

    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, U>(T arg1, U arg2);

    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    //This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
    public sealed class MessengerHelper : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //Clean up eventTable every time a new level loads.
//        public void OnLevelWasLoaded(int unused)
//        {
//            Messenger.Cleanup();
//        }
    }
}