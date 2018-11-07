using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotHandler
{

    private static SnapshotHandler _instance;
    

    
    
    private SortedList<long, PlayerSnapshot> snapshotBuffer;
    private long start;
    private long end;

    private SnapshotHandler()
    {
        snapshotBuffer = new SortedList<long, PlayerSnapshot>();
        
    }

    public static SnapshotHandler GetInstance()
    {
        return _instance ?? (_instance = new SnapshotHandler());
    }

    public void ReceiveSnapshot(PlayerSnapshot snapshot)
    {
        if (end < snapshot.frameNumber)
        {
            snapshotBuffer.Add(snapshot.frameNumber,snapshot);
        }
    }

    public bool ready()
    {
        return snapshotBuffer.Count > 2;
    }

    public PlayerSnapshot getSnapshot(long frame, int subframe)
    {
        try
        {
            if (end < frame)
            {
                snapshotBuffer.Remove(start);
                start = end;
                bool changed = false;
                for (int i = 1; i < GlobalSettings.BUFFERWINDOW && !changed; i++)
                {
                    if (snapshotBuffer.ContainsKey(start + i))
                    {
                        long aux =start + i;
                        if (frame <= aux)
                        {
                            end = start + i;
                            changed = true;
                        }
                        else if (start < aux)
                        {
                            snapshotBuffer.Remove(start);
                            start = aux;
                        }
                    }
                }
            }

            return interpolate(snapshotBuffer[start], snapshotBuffer[end], frame, subframe);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public PlayerSnapshot interpolate(PlayerSnapshot past, PlayerSnapshot future, long frame, int subframe)
    {
        long totalFrames = (future.frameNumber - past.frameNumber) * GlobalSettings.PrintingSubFrameRate;
        PlayerSnapshot ps = new PlayerSnapshot(past.id);
        long percentage = ((frame - past.frameNumber) * subframe + subframe)/ (totalFrames*subframe);
        ps.rotation = Quaternion.Lerp(past.rotation, future.rotation, percentage);
        ps.position = Vector3.Lerp(past.position, future.position, percentage);
        ps.Health = past.Health;
        ps.Invulnerable = past.Invulnerable;
        return ps;
    }

    public void updatePlayer(PlayerSnapshot ps)
    {
        if (ps != null)
        {
            Player p = GameObject.Find("Player").GetComponent<Player>();
            p.Health = ps.Health;
            p.Invulnerable = ps.Invulnerable;
            p.gameObject.transform.position = ps.position;
            p.gameObject.transform.rotation = ps.rotation;
        }
    }


}
