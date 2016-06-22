using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class boidsController : MonoBehaviour
{
	public float minVelocity = 5;
	public float maxVelocity = 20;
	public float randomness = 1;
	public int flockSize = 20;
	public GameObject prefab;
	private float radius;
	public float maxAcceleration;
	public Collider2D coll;
	private GameObject[] boids;
	private Rigidbody2D[] boidsRigidBody;
	private boidFlocking[] boidsFlocking;
	public Vector2 flockCenter;
	public Vector2 flockVelocity;

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
//			boidsRigidBody [i].AddForce (new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode2D.Force);
			boids[i] = boid;
		}
	}

	void Update ()
	{
		Vector2 theCenter = Vector2.zero;
		Vector2 theVelocity = Vector2.zero;

		for (int i = 0; i < boids.Length; i++)
		{
			if (boids [i].activeSelf) 
			{
				theCenter = theCenter + new Vector2 (boids [i].transform.localPosition.x, boids [i].transform.localPosition.y);
				theVelocity = theVelocity + boidsRigidBody [i].velocity;
			}
		}

		flockCenter = theCenter/(flockSize);
		flockVelocity = theVelocity/(flockSize);
		for (int i = 0; i < boids.Length; i++)
		{
			for (int j = 0; j < boids.Length; j++) {
				if (!boidsFlocking [i].alive && boidsFlocking[i].isActiveAndEnabled) 
				{
					flockSize--;
					boidsFlocking [i].gameObject.SetActive (false);
				}
//				if (i != j && boids[i].activeSelf && boids[j].activeSelf && !boidsFlocking[i].stun && !boidsFlocking[j].stun)
//				{
//					float distance = Vector2.Distance (boids [j].transform.localPosition, boids [i].transform.localPosition);
////					boidsRigidBody [j].velocity += boidsRigidBody [j].velocity.normalized * Time.deltaTime;
//
//
//					if (distance < 5f) 
//					{
////						Debug.Log ("distance = " + distance + ", radius = " + radius);
////						boidsRigidBody [j].velocity += (boidsRigidBody [i].velocity - boidsRigidBody [j].velocity) * Time.deltaTime;
//						if (distance < radius)
//						{
//							float avoidX = boids[i].transform.localPosition.x - boids[j].transform.localPosition.x;
//							float avoidY = boids [i].transform.localPosition.y - boids [j].transform.localPosition.y;
//							float angle = Mathf.Atan2 (avoidY, avoidX);
//							boidsFlocking[j].dispatch = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
//						}
//						else if (distance >= radius)
//							boidsFlocking [j].dispatch += (flockVelocity - boidsRigidBody [j].velocity) * Time.deltaTime;
//						else
//							boidsFlocking [j].dispatch = new Vector2 (0.0f, 0.0f);
//					}
//				}
			}
		}
	}
}