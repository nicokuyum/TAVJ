using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : MonoBehaviour {

	
	public int id;
	public int MaxHealth;
	public int Health;
	public bool Invulnerable;
	
	public float time;
	public GameObject hitFeed;
	
	// Use this for initialization
	void Start ()
	{
		Health = GlobalSettings.MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if (Health <= 0)
		{
			this.gameObject.SetActive(false);
		}
	}

	public void ProvideHitFeedback()
	{
		GameObject go = Instantiate(hitFeed, gameObject.transform.position, Quaternion.LookRotation(Vector3.up));
		Destroy(go, 5.0f);
	}
}
