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
                ps.position.x = ps.position.x + GlobalSettings.speed * deltaTime;
                break;
            case PlayerAction.StartMoveRight:
                ps.position.z = ps.position.z + GlobalSettings.speed * deltaTime;
                break;
            case PlayerAction.StartMoveBack:
                ps.position.x = ps.position.x - GlobalSettings.speed * deltaTime;
                break;
            case PlayerAction.StartMoveLeft:
                ps.position.z = ps.position.z - GlobalSettings.speed * deltaTime;
                break;
            case PlayerAction.Shoot:
                break;
            default:
                throw new NotImplementedException();
                break;
        }
        //Debug.Log("La posicion ahora es " + ps.position.x);
    }
    
}
