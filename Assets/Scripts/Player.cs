using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Player : MonoBehaviour
{

	public int id;
	public int MaxHealth;
	public int Health;
	public bool Invulnerable;

	private Queue<PlayerInputMessage> actions = new Queue<PlayerInputMessage>();
	
	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {
//		Debug.Log ("Health = " + Health);
	}

	// Se llama antes del primer update (siempre despues de awake)
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveForward));
		} else if (Input.GetKeyUp(KeyCode.W))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveForward));
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveLeft));
		} else if (Input.GetKeyUp(KeyCode.A))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveLeft));
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveBack));
		} else if (Input.GetKeyUp(KeyCode.S))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveBack));
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveRight));
		} else if (Input.GetKeyUp(KeyCode.D))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveRight));
		}

		if (Input.GetMouseButtonDown(0))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.Shoot));
		}
	}

	public Queue<PlayerInputMessage> getActions()
	{
		return actions;
	}

	public override string ToString()
	{
		return "Health: " + Health + "\nInvulnerable: " + Invulnerable + " 		Position: " +
		       this.gameObject.transform.position;
	}
}
