using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encoder {
    
    public int GetBitsRequired(long value) {
        int bitsRequired = 0;
        while (value > 0) {
            bitsRequired++;
            value >>= 1;
        }
        return bitsRequired;
    }
    
    protected int BitLength(int a)
    {
        int counter = 1;
        while (Math.Pow(2,counter) < a){ counter++; }
        return counter;
    }
}
