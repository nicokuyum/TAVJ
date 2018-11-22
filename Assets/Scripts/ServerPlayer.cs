using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : MonoBehaviour {

	
	public int id;
	public int MaxHealth;
	public int Health;
	public bool Invulnerable;
	
	public float time;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Health <= 0)
		{
			this.gameObject.SetActive(false);
		}
	}
}
