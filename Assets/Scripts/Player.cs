using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private GameObject camera;
	public GameObject grenadePrefab;
	public ParticleSystem flash;
	
	public int id;
	public int Health;
	public bool Invulnerable;
	public float grenadeCooldown;
	
	public float time;
	private float acumTime;
	private float cooldown;

	public List<GameMessage> toSend = new List<GameMessage>();
	private Queue<PlayerInputMessage> actions = new Queue<PlayerInputMessage>();
	private HashSet<PlayerAction> frameActions = new HashSet<PlayerAction>();
	
	public GameObject dazzle;
	public ParticleSystem muzzleFlash;
	
	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {

	}

	// Se llama antes del primer update (siempre despues de awake)
	// Use this for initialization
	void Start () {
		camera = GameObject.Find("Camera");
		cooldown = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		time += Time.deltaTime;
		acumTime += Time.deltaTime;
		cooldown -= Time.deltaTime;
		
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
			Shoot();
			toSend.Add(new PlayerInputMessage(PlayerAction.Shoot, time, true));
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			if (cooldown <= 0)
			{
				GameObject go = Instantiate(grenadePrefab);
				go.GetComponent<Grenade>().Launch(transform.position, transform.forward);
				toSend.Add(new GrenadeLaunchMessage(transform.position, transform.forward, time, true));
				cooldown = grenadeCooldown;
			}
		}

		/*if (acumTime >= (1.0f / GlobalSettings.Fps))
		{
			acumTime -= (1.0f / GlobalSettings.Fps);
			foreach (var action in frameActions)
			{
				PlayerInputMessage msg = new PlayerInputMessage(action, time, true);
				toSend.Add(msg);
				if (SnapshotHandler.GetInstance().prediction)
				{
					actions.Enqueue(msg);
					applyAction(action);
				}
			}
			toSend.Add(new RotationMessage(this.gameObject.transform.eulerAngles));
			frameActions.Clear();
		}*/

		camera.transform.position = this.gameObject.transform.position;
		camera.transform.rotation = this.gameObject.transform.rotation;
	}

	void FixedUpdate()
	{
		foreach (var action in frameActions)
		{
			PlayerInputMessage msg = new PlayerInputMessage(action, time, true);
			toSend.Add(msg);
			if (SnapshotHandler.GetInstance().prediction)
			{
				actions.Enqueue(msg);
				applyAction(action);
			}
		}
		toSend.Add(new RotationMessage(this.gameObject.transform.eulerAngles));
		frameActions.Clear();
	}

	public void prediction(int lastId)
	{
		while (actions.Count > 0 && actions.Peek()._MessageId < lastId)
		{
			// Discard all messages that were applied by server
			actions.Dequeue();
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

	public void Shoot()
	{
		// Create muzzle flash
		Instantiate(flash, gameObject.transform);
		
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit,500)) {
			
			muzzleFlash.Play();
			if (hit.collider.CompareTag("serverplayer"))
			{
				ServerPlayer player = hit.collider.gameObject.GetComponent<ServerPlayer>();
				ShotMessage shot = new ShotMessage(player.id, time, true);
				toSend.Add(shot);
			}

			GameObject go = Instantiate(dazzle, hit.point, Quaternion.LookRotation(hit.normal));
			Destroy(go, 3.0f);
		}
	}
}
