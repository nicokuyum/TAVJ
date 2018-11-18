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

	public List<PlayerInputMessage> toSend = new List<PlayerInputMessage>();
	private Queue<PlayerInputMessage> actions = new Queue<PlayerInputMessage>();
	private HashSet<PlayerAction> frameActions = new HashSet<PlayerAction>();
	
	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {

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
		
		if (Input.GetKey(KeyCode.W))
		{
			frameActions.Add(PlayerAction.MoveForward);
		} 
		if (Input.GetKey(KeyCode.A))
		{
			frameActions.Add(PlayerAction.MoveLeft);
		} 
		if (Input.GetKey(KeyCode.S))
		{
			frameActions.Add(PlayerAction.MoveBack);
		} 
		if (Input.GetKey(KeyCode.D))
		{
			frameActions.Add(PlayerAction.MoveRight);
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
				PlayerInputMessage msg = new PlayerInputMessage(action, time);
				toSend.Add(msg);
				actions.Enqueue(msg);
				if (SnapshotHandler.GetInstance().prediction)
				{
					applyAction(action);
				}
				applyAction(action);
			}
			frameActions.Clear();
		}
		
		//this.gameObject.transform.rotation = this.gameObject.transform.GetChild(0).rotation;
	}

	public void prediction(int lastId, float deltaTime)
	{
		Debug.Log("In prediction function, last id is " + lastId);
		while (actions.Any() && actions.Peek()._MessageId < lastId)
		{
			// Discard all messages that were applied by server
			Debug.Log("Discarding action with id " + actions.Peek()._MessageId);
			actions.Dequeue();
		}
		
		Debug.Log("Actions size " + actions.Count);
		if (actions.Any())
		{
			Debug.Log("First action id is " + actions.Peek()._MessageId);
		}
		

		foreach (var actionMsg in actions)
		{
			applyAction(actionMsg.Action);
		}
	}

	public void applyAction(PlayerAction action)
	{
		switch (action)
		{
			case PlayerAction.MoveForward:
				gameObject.transform.Translate(Vector3.forward * GlobalSettings.speed * (1.0f / GlobalSettings.Fps));
				break;
			case PlayerAction.MoveRight:
				gameObject.transform.Translate(Vector3.right * GlobalSettings.speed * (1.0f / GlobalSettings.Fps));
				break;
			case PlayerAction.MoveBack:
				gameObject.transform.Translate(Vector3.back * GlobalSettings.speed * (1.0f / GlobalSettings.Fps));
				break;
			case PlayerAction.MoveLeft:
				gameObject.transform.Translate(Vector3.left * GlobalSettings.speed * (1.0f / GlobalSettings.Fps));
				break;
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
