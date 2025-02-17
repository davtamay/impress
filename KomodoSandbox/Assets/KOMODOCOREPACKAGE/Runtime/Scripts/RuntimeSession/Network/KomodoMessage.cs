using UnityEngine;
using System.Runtime.InteropServices;

//namespace Komodo.Runtime
//{
    // TODO(rob): move this to GlobalMessageManager.cs

    // Message System: WIP
    // to send a message
    // 1. pack a struct with the data you need
    // 2. serialize that struct
    // 3. pass the message `type` and the serialized struct in the constructor
    // 4. call the .Send() method
    // 5. write a handler and register it in the ProcessMessage function below
    // 6. this is still a hacky way to do it, so feel free to change/improve as you see fit. 
    [System.Serializable]
    public struct KomodoMessage
    {
        public string type;

        public string data;

        public int sendTo;
        //sendTo = -1 (To all), 0 (To all Except sender), clientID (to target clientID)
        public KomodoMessage(string type, string messageData, int sendTo = 0)
        {
            this.type = type;
            this.data = messageData;
            this.sendTo = sendTo;
        }

        public void Send()
        {

#if UNITY_WEBGL && !UNITY_EDITOR
             SocketIOJSLib.BrowserEmitMessage(this.type, this.data, this.sendTo);
#else
            var socketSim = SocketIOEditorSimulator.Instance;

            if (!socketSim)
            {
                Debug.LogWarning("No SocketIOEditorSimulator found");
            }

            socketSim.BrowserEmitMessage(this.type, this.data);
#endif
        }
    }
//}