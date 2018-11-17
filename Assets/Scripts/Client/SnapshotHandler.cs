using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapshotHandler
{

    private static SnapshotHandler _instance;

    public Player self;
    public Dictionary<int, Player> otherPlayers;
    
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
        try
        {
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
                Debug.Log("Update other");
                Player p = otherPlayers[pair.Key];
                p.Health = pair.Value.Health;
                p.Invulnerable = pair.Value.Invulnerable;
                p.gameObject.transform.position = pair.Value.position;
                p.gameObject.transform.rotation = pair.Value.rotation;
            }
            else
            {
                Debug.Log("Handling self");
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
        //p.gameObject.transform.rotation = playerSnapshot.rotation;
        if (prediction)
        {
            Debug.Log("Applying prediction");
            p.prediction(playerSnapshot.lastId, playerSnapshot._TimeStamp);
        }
    }



    public void ReapplyActions(Player p, float time)
    {
        HashSet<PlayerAction> last = null;
        float lt=0;
        
        foreach (KeyValuePair<float,HashSet<PlayerAction>> action in actions)
        {
            if (action.Key > time)
            {
                if (last != null)
                {
                    foreach (PlayerAction playerAction in last)
                    {
                        
                        Debug.Log("||||||||||||||||||||||||");
                        //Ojo aca, puede llegar a pifear
                        ApplyAction(p, playerAction,  action.Key-lt);
                    }
                }
                last = action.Value;
                lt = action.Key;
            }
            else
            {
                lt = action.Key;
                last = action.Value;
            }
        }

        List<float> toRemove = new List<float>();
        foreach (float key in actions.Keys)
        {
            if (key < time && key < lastActionTime)
            {
                toRemove.Add(key);
            }
        }
        foreach (float f in toRemove)
        {
            actions.Remove(f);
        }
        
    }

    public void ApplyAction(Player p,PlayerAction playerAction ,float time)
    {
        switch (playerAction)
        {
            case PlayerAction.MoveForward:
                p.gameObject.transform.Translate(Vector3.forward * GlobalSettings.speed * time);
                break;
            case PlayerAction.MoveRight:
                p.gameObject.transform.Translate(Vector3.right * GlobalSettings.speed * time);
                break;
            case PlayerAction.MoveBack:
                p.gameObject.transform.Translate(Vector3.back * GlobalSettings.speed * time);
                break;
            case PlayerAction.MoveLeft:
                p.gameObject.transform.Translate(Vector3.left * GlobalSettings.speed * time);
                break;
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
