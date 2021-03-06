﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompressingUtils  {

    public static void WriteTime(Compressor compressor, float time)
    {
        compressor.WriteFloat(time, GlobalSettings.MaxTime, GlobalSettings.MinTime, GlobalSettings.TimePrecision);
    }
    
    public static float GetTime(Decompressor decompressor)
    {
        return decompressor.GetFloat(GlobalSettings.MaxTime, GlobalSettings.MinTime, GlobalSettings.TimePrecision);
    }

    public static void WritePosition(Compressor compressor, Vector3 position)
    {
        compressor.WriteFloat(position.x, GlobalSettings.MaxPosition,
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        compressor.WriteFloat(position.y, GlobalSettings.MaxPosition,
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        compressor.WriteFloat(position.z, GlobalSettings.MaxPosition,
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
    }

    public static Vector3 GetPosition(Decompressor decompressor)
    {
        float x = decompressor.GetFloat(GlobalSettings.MaxPosition, 
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        float y = decompressor.GetFloat(GlobalSettings.MaxPosition, 
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        float z = decompressor.GetFloat(GlobalSettings.MaxPosition, 
            GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        return new Vector3(x,y,z);
    }
    
    public static void WriteRotation(Compressor compressor, Vector3 position)
    {
        compressor.WriteFloat(position.x, GlobalSettings.MaxAngle,
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
        compressor.WriteFloat(position.y, GlobalSettings.MaxAngle,
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
        compressor.WriteFloat(position.z, GlobalSettings.MaxAngle,
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
    }
    
    
    public static Vector3 GetRotation(Decompressor decompressor)
    {
        float x = decompressor.GetFloat(GlobalSettings.MaxAngle, 
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
        float y = decompressor.GetFloat(GlobalSettings.MaxAngle, 
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
        float z = decompressor.GetFloat(GlobalSettings.MaxAngle, 
            GlobalSettings.MinAngle, GlobalSettings.AnglePrecision);
        return new Vector3(x,y,z);
    }
}
