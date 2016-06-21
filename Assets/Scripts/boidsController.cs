using UnityEngine;
using System.Collections;

public class boidsController : MonoBehaviour
{
	public float minVelocity = 5;
	public float maxVelocity = 20;
	public float randomness = 1;
	public int flockSize = 20;
	public GameObject prefab;
	public GameObject chasee;
	private float radius;
	public Collider2D coll;
	private GameObject[] boids;
	private Rigidbody2D[] boidsRigidBody;
	private boidFlocking[] boidsFlocking;

	void Start()
	{
		coll = GetComponent<Collider2D> ();
		boids = new GameObject[flockSize];
		boidsRigidBody = new Rigidbody2D[flockSize];
		boidsFlocking = new boidFlocking[flockSize];
		for (var i = 0; i < flockSize; i++)
		{
			Vector3 position = new Vector3 (
				Random.value * coll.bounds.size.x,
				Random.value * coll.bounds.size.y,
				Random.value * coll.bounds.size.z
			) - coll.bounds.extents;

			GameObject boid = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
			boid.transform.parent = transform;
			boid.transform.localPosition = position;
			radius = boid.GetComponent<CircleCollider2D> ().radius;
			boid.GetComponent<boidFlocking>().SetController (gameObject);
			boidsRigidBody [i] = boid.gameObject.GetComponent<Rigidbody2D> ();
			boidsFlocking [i] = boid.gameObject.GetComponent<boidFlocking> ();
			boidsRigidBody [i].AddForce (new Vector2(Random.Range (-0.5f, 0.5f), Random.Range (-0.5f, 0.5f)), ForceMode2D.Force);
			boids[i] = boid;
		}
	}

	void Update ()
	{
		for (int i = 0; i < boids.Length; i++)
		{
			for (int j = 0; j < boids.Length; j++) {
				if (!boidsFlocking [i].alive)
					boidsFlocking [i].gameObject.SetActive (false);
				if (i != j && boids[i].activeSelf && boids[j].activeSelf && !boidsFlocking[i].stun && !boidsFlocking[j].stun)
				{
					float distance = Vector2.Distance (boids [j].transform.localPosition, boids [i].transform.localPosition);
					if (distance < 5f) {
//						Debug.Log ("distance = " + distance + ", radius = " + radius);
						boidsRigidBody [j].velocity += (boidsRigidBody [i].velocity - boidsRigidBody [j].velocity) * 0.01f;
						if (distance >= radius)
							boidsFlocking [j].dispatch += (boidsRigidBody [i].velocity - boidsRigidBody [j].velocity) * 0.001f;
						else
							boidsFlocking [j].dispatch = new Vector2 (0.0f, 0.0f);
					}
				}
			}
		}
	}
}