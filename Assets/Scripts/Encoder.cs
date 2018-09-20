using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encoder {
    
    protected const int _minChar = 33;
    protected const int _maxChar = 126;
    protected const int _maxStringLength = 20;
    
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
