using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotHandler
{

    private static SnapshotHandler _instance;
    
    
    private SortedList<float, Dictionary<int, PlayerSnapshot>> worldSnapshots;
    
    private SortedList<float, PlayerSnapshot> snapshotBuffer;
    
    private float start;
    private float end;

    private SnapshotHandler(float start, float end)
    {
        snapshotBuffer = new SortedList<float, PlayerSnapshot>();
        
    }

    public static SnapshotHandler GetInstance()
    {
        return _instance ?? (_instance = new SnapshotHandler(-1,-1));
    }

    public void ReceiveSnapshot(PlayerSnapshot snapshot)
    {
        if (end < snapshot._TimeStamp)
        {
            snapshotBuffer.Add(snapshot._TimeStamp, snapshot);
        }
    }

    public bool ready()
    {
        return snapshotBuffer.Count > GlobalSettings.BufferWindow;
    }

    public PlayerSnapshot getSnapshot(float time)
    {
        try
        {
            if (end < time)
            {
                start = end;
                bool endFound = false;
                foreach(float f in snapshotBuffer.Keys)
                {
                    if (!endFound)
                    {
                        if (f > time)
                        {
                            endFound = true;
                            end = f;
                        }
                        else
                        {
                            start = f;
                        }
                    }
                }

                if (!endFound)
                {
                    //TODO Could throw an exception or handle it more elegantly
                    return null;
                }
                
            }
            deleteOldSnapshots();

            return interpolate(snapshotBuffer[start], snapshotBuffer[end], time);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }


    public PlayerSnapshot interpolate(PlayerSnapshot past, PlayerSnapshot future, float time)
    {
        float timeRatio = (time - past._TimeStamp) / (future._TimeStamp - past._TimeStamp);
        PlayerSnapshot interpolatedPlayerSnapshot = new PlayerSnapshot(past.id);
        interpolatedPlayerSnapshot._TimeStamp = time;
        interpolatedPlayerSnapshot.Health = past.Health;
        interpolatedPlayerSnapshot.Invulnerable = past.Invulnerable;
        interpolatedPlayerSnapshot.position = Vector3.Lerp(past.position, future.position, timeRatio);
        interpolatedPlayerSnapshot.rotation = Quaternion.Lerp(past.rotation, future.rotation, timeRatio);
        return interpolatedPlayerSnapshot;
    }

    public void updatePlayer(PlayerSnapshot ps)
    {
        if (ps != null)
        {
            Debug.Log("Updating player snapshot");
            Player p = GameObject.Find("Player").GetComponent<Player>();
            p.Health = ps.Health;
            p.Invulnerable = ps.Invulnerable;
            p.gameObject.transform.position = ps.position;
            p.gameObject.transform.rotation = ps.rotation;
        }
    }

    public Dictionary<int, PlayerSnapshot> interpolateWorld(Dictionary<int, PlayerSnapshot> past,
        Dictionary<int, PlayerSnapshot> future, float time)
    {
        Dictionary<int, PlayerSnapshot> interpolatedWorld = new Dictionary<int, PlayerSnapshot>();
        foreach (KeyValuePair<int,PlayerSnapshot> keyValuePair in past)
        {
            if (future.ContainsKey(keyValuePair.Key))
            {
                PlayerSnapshot interpolatedPlayerSnapshot = interpolate(keyValuePair.Value,
                    future[keyValuePair.Key], time);
                    
                interpolatedWorld.Add(keyValuePair.Key,interpolatedPlayerSnapshot);
            }
            
        }

        return interpolatedWorld;
    }

    public void deleteOldSnapshots()
    {
        List<float> messagesToDelete = new List<float>();
        
        foreach (float snapshotBufferKey in snapshotBuffer.Keys)
        {
            if (snapshotBufferKey < start)
            {
                messagesToDelete.Add(snapshotBufferKey);
            }
        }
        
        foreach (float f in messagesToDelete)
        {
            snapshotBuffer.Remove(f);
        }
        
    }



}
