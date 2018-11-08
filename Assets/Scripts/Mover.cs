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

    public void ApplyAction(PlayerSnapshot ps, PlayerAction action, float deltaTime)
    {
        
        switch (action)
        {
            case PlayerAction.StartMoveForward:
                ps.player.transform.Translate(Vector3.forward * GlobalSettings.speed * deltaTime);
                break;
            case PlayerAction.StartMoveRight:
                ps.player.transform.Translate(Vector3.right * GlobalSettings.speed * deltaTime);
                break;
            case PlayerAction.StartMoveBack:
                ps.player.transform.Translate(Vector3.back * GlobalSettings.speed * deltaTime);
                break;
            case PlayerAction.StartMoveLeft:
                ps.player.transform.Translate(Vector3.left * GlobalSettings.speed * deltaTime);
                break;
            case PlayerAction.Shoot:
                break;
            default:
                throw new NotImplementedException();
                break;
        }
        ps.position = ps.player.transform.position;
        //Debug.Log("La posicion ahora es " + ps.position.x);
    }
}
