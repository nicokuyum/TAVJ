using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType {

    CONNECT,
    INPUT,
    ACK
    
}


static class MessageTypeMethods
{
    public static bool isReliable(this MessageType mt)
    {
        switch (mt)
        {
                case MessageType.CONNECT:
                    return true;
                default:
                    return false;
        }
    }
}