using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
	public float lifeTime = 5.0f;

	private float strength = 100.0f;
	private float timer;

	void Start()
	{
		this.timer = 0.0f;
	}

	void Update()
	{
		timer += Time.deltaTime; 
		if (timer > lifeTime)
		{
			// EXPLODE
			Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{
		GetComponent<Rigidbody>().AddForce(Physics.gravity * 0.1f);
	}

	public void Launch (Vector3 direction)
	{
		GetComponent<Rigidbody>().AddForce((direction + new Vector3(0, 0.3f, 0)) * strength);
	}
}
