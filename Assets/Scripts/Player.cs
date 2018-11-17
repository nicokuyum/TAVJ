using System;
using System.Collections.Generic;
using System.Linq;
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
	private float acumTime;

	private Queue<PlayerInputMessage> actions = new Queue<PlayerInputMessage>();
	private HashSet<PlayerAction> frameActions = new HashSet<PlayerAction>();
	
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
		acumTime += Time.deltaTime;
		
	
		if (Input.GetKeyDown(KeyCode.W))
		{
			frameActions.Add(PlayerAction.MoveForward);
			//actions.Enqueue(new PlayerInputMessage(PlayerAction.MoveForward, time));
		} 
		if (Input.GetKeyDown(KeyCode.A))
		{
			frameActions.Add(PlayerAction.MoveLeft);
			//actions.Enqueue(new PlayerInputMessage(PlayerAction.MoveLeft, time));
		} 
		if (Input.GetKeyDown(KeyCode.S))
		{
			frameActions.Add(PlayerAction.MoveBack);
			//actions.Enqueue(new PlayerInputMessage(PlayerAction.MoveBack, time));
		} 
		if (Input.GetKeyDown(KeyCode.D))
		{
			frameActions.Add(PlayerAction.MoveRight);
			//actions.Enqueue(new PlayerInputMessage(PlayerAction.MoveRight, time));
		} 

		if (Input.GetMouseButtonDown(0))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.Shoot, time));
		}

		if (acumTime >= (1.0f / GlobalSettings.Fps))
		{
			acumTime -= (1.0f / GlobalSettings.Fps);
			foreach (var action in frameActions)
			{
				actions.Enqueue(new PlayerInputMessage(action, time));
			}
			frameActions.Clear();
		}
		
		//this.gameObject.transform.rotation = this.gameObject.transform.GetChild(0).rotation;
	}

	public void prediction(int lastId, float deltaTime)
	{
		while (actions.Any() && actions.Peek()._MessageId < lastId)
		{
			// Discard all messages that were applied by server
			actions.Dequeue();
		}

		foreach (var actionMsg in actions)
		{
			switch (actionMsg.Action)
			{
				case PlayerAction.MoveForward:
					gameObject.transform.Translate(Vector3.forward * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.MoveRight:
					gameObject.transform.Translate(Vector3.right * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.MoveBack:
					gameObject.transform.Translate(Vector3.back * GlobalSettings.speed * deltaTime);
					break;
				case PlayerAction.MoveLeft:
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
