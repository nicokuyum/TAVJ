using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Player : MonoBehaviour
{

	private float yaw = 0.0f;
	private float pitch = 0.0f;
	
	public int id;
	public int MaxHealth;
	public int Health;
	public bool Invulnerable;
	
	public float time;

	private HashSet<PlayerAction> liveActions = new HashSet<PlayerAction>();

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
		time += Time.deltaTime;
		prediction(Time.deltaTime);
		
		if (Input.GetKeyDown(KeyCode.W))
		{
			liveActions.Add(PlayerAction.StartMoveForward);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveForward, time));
		} else if (Input.GetKeyUp(KeyCode.W))
		{
			liveActions.Remove(PlayerAction.StartMoveForward);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveForward, time));
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			liveActions.Add(PlayerAction.StartMoveLeft);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveLeft, time));
		} else if (Input.GetKeyUp(KeyCode.A))
		{
			liveActions.Remove(PlayerAction.StartMoveLeft);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveLeft, time));
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			liveActions.Add(PlayerAction.StartMoveBack);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveBack, time));
		} else if (Input.GetKeyUp(KeyCode.S))
		{
			liveActions.Remove(PlayerAction.StartMoveBack);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveBack, time));
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			liveActions.Add(PlayerAction.StartMoveRight);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveRight, time));
		} else if (Input.GetKeyUp(KeyCode.D))
		{ 
			liveActions.Remove(PlayerAction.StartMoveRight);
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveRight, time));
		}

		if (Input.GetMouseButtonDown(0))
		{
			
			actions.Enqueue(new PlayerInputMessage(PlayerAction.Shoot, time));
		}
		
		//this.gameObject.transform.rotation = this.gameObject.transform.GetChild(0).rotation;
	}

	public void prediction(float deltaTime)
	{
		foreach (PlayerAction action in liveActions)
		{
			switch (action)
			{
				case PlayerAction.StartMoveForward:
					gameObject.transform.Translate(Vector3.forward * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.StartMoveRight:
					gameObject.transform.Translate(Vector3.right * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.StartMoveBack:
					gameObject.transform.Translate(Vector3.back * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.StartMoveLeft:
					gameObject.transform.Translate(Vector3.left * GlobalSettings.speed * deltaTime);
					break;
			}
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
