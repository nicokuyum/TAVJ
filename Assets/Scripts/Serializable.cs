using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Serializable<T>
{
    byte[] serialize();
    void deserialize(byte[] data);
}
