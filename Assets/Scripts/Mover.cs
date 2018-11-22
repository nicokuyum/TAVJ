using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Object = UnityEngine.Object;

public class Mover {

    private static readonly Object LockObject = new Object();
    private static Mover _instance;
    
    private Mover()
    {
    }

    public static Mover GetInstance()
    {
        return _instance ?? (_instance = new Mover());
    }

    public void ApplyAction(PlayerSnapshot ps, PlayerAction action, float duration)
    {
        
        switch (action)
        {
            case PlayerAction.MoveForward:
                ps.player.transform.Translate(Vector3.forward * GlobalSettings.speed * duration);
                break;
            case PlayerAction.MoveRight:
                ps.player.transform.Translate(Vector3.right * GlobalSettings.speed * duration);
                break;
            case PlayerAction.MoveBack:
                ps.player.transform.Translate(Vector3.back * GlobalSettings.speed * duration);
                break;
            case PlayerAction.MoveLeft:
                ps.player.transform.Translate(Vector3.left * GlobalSettings.speed * duration);
                break;
            case PlayerAction.Shoot:
                break;
            default:
                throw new NotImplementedException();
                break;
        }
        ps.position = ps.player.transform.position;
    }
}
