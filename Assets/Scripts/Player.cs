using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Player : MonoBehaviour, Serializable<Player>
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
		} else if (Input.GetKeyDown(KeyCode.W))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveForward));
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveLeft));
		} else if (Input.GetKeyDown(KeyCode.A))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveLeft));
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveBack));
		} else if (Input.GetKeyDown(KeyCode.S))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveBack));
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StartMoveRight));
		} else if (Input.GetKeyDown(KeyCode.D))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.StopMoveRight));
		}

		if (Input.GetMouseButtonDown(0))
		{
			actions.Enqueue(new PlayerInputMessage(PlayerAction.Shoot));
		}
	}

	public byte[] serialize()
	{
		Compressor compressor = new Compressor();
		
		Vector3 pos = this.transform.position;
		Quaternion rotation = this.transform.rotation;
		//compressor.WriteNumber(frameNumber, compressor.GetBitsRequired(3600 * (long)fps ));
		compressor.WriteNumber(this.Health, compressor.GetBitsRequired(this.MaxHealth));
		compressor.PutBit(this.Invulnerable);
		compressor.WriteFloat(pos.x, 100, 0, 0.1f);
		compressor.WriteFloat(pos.y, 100, 0, 0.1f);
		compressor.WriteFloat(pos.z, 100, 0, 0.1f);
//		compressor.WriteFloat(rotation.w);
		return compressor.GetBuffer();
	}

	public void deserialize(byte[] data)
	{
		Vector3 pos = new Vector3();
		Decompressor decompressor = new Decompressor(data);
		//this.frameNumber = decompressor.GetNumber(3600 * (long) fps);
		this.Health = decompressor.GetNumber(this.MaxHealth);
		this.Invulnerable = decompressor.GetBoolean();
		pos.x = decompressor.GetFloat(100, 0, 0.1f);
		pos.y = decompressor.GetFloat(100, 0, 0.1f);
		pos.z = decompressor.GetFloat(100, 0, 0.1f);
		this.gameObject.transform.position = pos;
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
