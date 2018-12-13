using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
	public float lifeTime = 5.0f;
	public float radius = 4.0f;
	public int damage = 30;

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
			Explode();
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

	private void Explode()
	{
		Collider[] overlaps = Physics.OverlapSphere(transform.position, radius);
		foreach (var c in overlaps)
		{
			ServerPlayer victim = c.gameObject.GetComponent<ServerPlayer>();
			if (victim != null)
			{
				victim.Health -= damage;
			}
		}
	}
}
