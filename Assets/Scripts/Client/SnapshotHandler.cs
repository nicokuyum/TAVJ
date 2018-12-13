using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapshotHandler
{

    private static SnapshotHandler _instance;

    public Player self;
    public Dictionary<int, ServerPlayer> otherPlayers;
    
    private SortedList<float, Dictionary<int, PlayerSnapshot>> worldSnapshots;
    
    private SortedList<float, PlayerSnapshot> snapshotBuffer;

    private SortedList<float, HashSet<PlayerAction>> actions;
    private float lastActionTime;
    
    private float start;
    private float end;

    public bool prediction;

    private SnapshotHandler(float start, float end)
    {
        worldSnapshots = new SortedList<float, Dictionary<int, PlayerSnapshot>>();
        snapshotBuffer = new SortedList<float, PlayerSnapshot>();
        actions = new SortedList<float, HashSet<PlayerAction>>();
        lastActionTime = -1;
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
    }

    public bool ready()
    {
        return worldSnapshots.Count > GlobalSettings.BufferWindow;
    }

    public Dictionary<int,PlayerSnapshot> getSnapshot(float time)
    {
        time -= 3.0f * 1.0f / (GlobalSettings.ServerSendRate);
        try
        {
            if (end < time)
            {
                start = end;
                bool endFound = false;
                foreach(float f in worldSnapshots.Keys)
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
        float timeRatio = (time - past._TimeStamp) / (future._TimeStamp - past._TimeStamp);
        PlayerSnapshot interpolatedPlayerSnapshot = new PlayerSnapshot(past.id);
        interpolatedPlayerSnapshot._TimeStamp = time;
        interpolatedPlayerSnapshot.Health = past.Health;
        interpolatedPlayerSnapshot.Invulnerable = past.Invulnerable;
        interpolatedPlayerSnapshot.position = Vector3.Lerp(past.position, future.position, timeRatio);
        Vector3 interpolatedRotation = Vector3.Lerp(past.rotation.eulerAngles, future.rotation.eulerAngles, timeRatio);
        interpolatedPlayerSnapshot.rotation = Quaternion.Euler(interpolatedRotation);
        return interpolatedPlayerSnapshot;
    }

    public void updatePlayer(Dictionary<int, PlayerSnapshot> world)
    {
        foreach (KeyValuePair<int,PlayerSnapshot> pair in world)
        {
            if (otherPlayers.ContainsKey(pair.Key))
            {
                ServerPlayer p = otherPlayers[pair.Key];
                if (p.Health > pair.Value.Health)
                {
                    p.ProvideHitFeedback();
                }
                p.Health = pair.Value.Health;
                p.Invulnerable = pair.Value.Invulnerable;
                p.gameObject.transform.position = pair.Value.position;
                p.gameObject.transform.rotation = pair.Value.rotation;
            }
            else
            {
                handleSelfSnapshot(pair.Value);
            }
        }
    }

    public void handleSelfSnapshot(PlayerSnapshot playerSnapshot)
    {
        Player p = self;
        p.Health = playerSnapshot.Health;
        p.Invulnerable = playerSnapshot.Invulnerable;
        p.gameObject.transform.position = playerSnapshot.position;
        if (prediction)
        {
            p.prediction(playerSnapshot.lastId);
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
                if (self.id == keyValuePair.Key)
                {
                    interpolatedWorld.Add(keyValuePair.Key, future[keyValuePair.Key]);
                }
                else
                {
                    PlayerSnapshot interpolatedPlayerSnapshot = interpolate(keyValuePair.Value,
                        future[keyValuePair.Key], time);   
                    interpolatedWorld.Add(keyValuePair.Key,interpolatedPlayerSnapshot);
                }    
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



    public void AddActionForPrediction(HashSet<PlayerAction> input, float time)
    {
        actions.Add(time, new HashSet<PlayerAction>(input));
    }


}
