using Gamekit3D.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kk : MonoBehaviour, IMessageReceiver
{
    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        Debug.Log("Send AUA");
    }
}
