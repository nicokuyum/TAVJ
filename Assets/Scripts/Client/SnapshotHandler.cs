using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotHandler
{

    private static SnapshotHandler _instance;

    public Player self;
    public Dictionary<int, Player> otherPlayers;
    
    private SortedList<float, Dictionary<int, PlayerSnapshot>> worldSnapshots;
    
    private SortedList<float, PlayerSnapshot> snapshotBuffer;
    
    private float start;
    private float end;

    private SnapshotHandler(float start, float end)
    {
        worldSnapshots = new SortedList<float, Dictionary<int, PlayerSnapshot>>();
        snapshotBuffer = new SortedList<float, PlayerSnapshot>();
        this.start = start;
        this.end = end;
    }

    public static SnapshotHandler GetInstance()
    {
        return _instance ?? (_instance = new SnapshotHandler(-1,-1));
    }

    public void ReceiveSnapshot(Dictionary<int, PlayerSnapshot> worldSnap, float time)
    {
        if (end < time)
        {
            worldSnapshots.Add(time, worldSnap);
        }
        /*if (end < snapshot._TimeStamp)
        {
            snapshotBuffer.Add(snapshot._TimeStamp, snapshot);
        }*/
    }

    public bool ready()
    {
        return worldSnapshots.Count > GlobalSettings.BufferWindow;
    }

    public Dictionary<int,PlayerSnapshot> getSnapshot(float time)
    {
        try
        {
            //Debug.Log("In getSnapshot (end,time) " + end + "," + time);
            if (end < time)
            {
                start = end;
                bool endFound = false;
                foreach(float f in worldSnapshots.Keys)
                {
                    //Debug.Log("Checking key " + f);
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
                    //Debug.Log("End not found");
                    return null;
                }
                
            }
            deleteOldSnapshots();

            return interpolateWorld(worldSnapshots[start], worldSnapshots[end], time);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }


    public PlayerSnapshot interpolate(PlayerSnapshot past, PlayerSnapshot future, float time)
    {
        //Debug.Log("past: " + past._TimeStamp + " - future: " + future._TimeStamp + " - time: " + time);
        float timeRatio = (time - past._TimeStamp) / (future._TimeStamp - past._TimeStamp);
        PlayerSnapshot interpolatedPlayerSnapshot = new PlayerSnapshot(past.id);
        interpolatedPlayerSnapshot._TimeStamp = time;
        interpolatedPlayerSnapshot.Health = past.Health;
        interpolatedPlayerSnapshot.Invulnerable = past.Invulnerable;
        interpolatedPlayerSnapshot.position = Vector3.Lerp(past.position, future.position, timeRatio);
        //interpolatedPlayerSnapshot.rotation = Quaternion.Lerp(past.rotation, future.rotation, timeRatio);
        return interpolatedPlayerSnapshot;
    }

    public void updatePlayer(Dictionary<int, PlayerSnapshot> world)
    {
        foreach (KeyValuePair<int,PlayerSnapshot> pair in world)
        {
            if (otherPlayers.ContainsKey(pair.Key))
            {
                Player p = otherPlayers[pair.Key];
                p.Health = pair.Value.Health;
                p.Invulnerable = pair.Value.Invulnerable;
                p.gameObject.transform.position = pair.Value.position;
                p.gameObject.transform.rotation = pair.Value.rotation;
            }
            else
            {
                handleSelfSnapshot(pair.Key, pair.Value);
            }
        }
    }

    public void handleSelfSnapshot(int id, PlayerSnapshot playerSnapshot)
    {
        Player p = self;
        p.Health = playerSnapshot.Health;
        p.Invulnerable = playerSnapshot.Invulnerable;
        p.gameObject.transform.position = playerSnapshot.position;
        //p.gameObject.transform.rotation = playerSnapshot.rotation;
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
        
        foreach (float worldSnapshotKey in worldSnapshots.Keys)
        {
            if (worldSnapshotKey < start)
            {
                messagesToDelete.Add(worldSnapshotKey);
            }
        }
        
        foreach (float f in messagesToDelete)
        {
            snapshotBuffer.Remove(f);
        }
        
    }



}
