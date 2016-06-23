using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class boidsController : MonoBehaviour
{
	public float maxVelocity = 20;
	public float randomness = 1;
	public int flockSize = 20;
	private int maxFlockSize;
	public GameObject prefab;
	public float radius;
	public float maxAcceleration;
	public Collider2D coll;
	private GameObject[] boids;
	private Rigidbody2D[] boidsRigidBody;
	private boidFlocking[] boidsFlocking;
	public Vector2 flockCenter;
	public Vector2 flockVelocity;
	public bool raycast = false;
	public Text counter;

	void Start()
	{
		maxFlockSize = flockSize;
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
			boids[i] = boid;
		}
		counter.text = "";
		if (raycast)
			StartCoroutine ("raycastSearch");
	}

	IEnumerator raycastSearch()
	{
		while (true) {
			int maxIndex = 0;
			float maxValue = 0;
			for (int i = 0; i < 5; i++) {
				Debug.DrawRay (flockCenter, 
					new Vector2 ((flockVelocity.x + 1.0f) * 20 * Mathf.Cos ((i * (360 / 5)) * Mathf.Deg2Rad),
						(flockVelocity.y + 1.0f) * 20 * Mathf.Sin ((i * (360 / 5)) * Mathf.Deg2Rad)));
				RaycastHit2D[] hits = Physics2D.RaycastAll (flockCenter, 
					new Vector2 ((flockVelocity.x + 1.0f) * 20 * Mathf.Cos ((i * (360 / 5)) * Mathf.Deg2Rad),
						(flockVelocity.y + 1.0f) * 20 * Mathf.Sin ((i * (360 / 5) + 360) * Mathf.Deg2Rad)));
				foreach (RaycastHit2D hit in hits) {
					if (hit.collider.tag != "agents" && hit.distance > maxValue) {
						Debug.Log (hit.collider.tag);
						maxValue = hit.distance;
						maxIndex = i;
					}
				}
				yield return null;
				//			yield return new WaitForSeconds (2);
			}
			flockCenter += new Vector2 (flockVelocity.x + Mathf.Cos ((maxIndex * (360 / 5)) * Mathf.Deg2Rad),
				flockVelocity.y + Mathf.Sin ((maxIndex * (360 / 5)) * Mathf.Deg2Rad));
			//			rigidbody.flockVelocity += new Vector2 (rigidbody.flockVelocity.x + Mathf.Cos ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad),
			//				rigidbody.flockVelocity.y + Mathf.Sin ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad));
			//		yield return null;
			Vector2 theCenter = Vector2.zero;
			Vector2 theVelocity = Vector2.zero;

			for (int i = 0; i < boids.Length; i++)
			{
				if (boids [i].activeSelf) 
				{
					theCenter = theCenter + new Vector2 (boids [i].transform.position.x, boids [i].transform.position.y);
					//				theVelocity = theVelocity + boidsRigidBody [i].velocity.normalized;
				}
			}

			flockCenter += theCenter/(flockSize);
			//		flockVelocity = theVelocity/(flockSize);
			yield return new WaitForSeconds (2);
		}
	}

	void Update ()
	{
		counter.text = "Count : " + flockSize + "/" + maxFlockSize;
		Vector2 theCenter = Vector2.zero;
//		Vector2 theVelocity = Vector2.zero;

		for (int i = 0; i < boids.Length; i++)
		{
			if (boids [i].activeSelf) 
			{
				theCenter = theCenter + new Vector2 (boids [i].transform.position.x, boids [i].transform.position.y);
				//				theVelocity = theVelocity + boidsRigidBody [i].velocity.normalized;
			}
		}

		flockCenter = theCenter/(flockSize);
		//		flockVelocity = theVelocity/(flockSize);
//		Vector2 direction = new Vector2(0.0f, 0.0f);
//		Vector3 coordinates = Camera.main.WorldToViewportPoint (transform.position);
//		foreach (attractor attract in attracts) {
//			Vector3 coordinatesAttract = Camera.main.WorldToViewportPoint (attract.transform.position);
//			direction += new Vector2( attract.force / Vector3.Distance(coordinatesAttract, coordinates) * (coordinatesAttract.x - coordinates.x),  attract.force / Vector3.Distance(coordinatesAttract, coordinates) * (coordinatesAttract.y - coordinates.y));
//		}
////		direction /= attracts.Length;
//		flockVelocity += direction * Time.deltaTime;
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
//					float distance = Vector2.Distance (boids [j].transform.position, boids [i].transform.position);
////					boidsRigidBody [j].velocity += boidsRigidBody [j].velocity.normalized * Time.deltaTime;
//
//
//					if (distance < 5f) 
//					{
////						Debug.Log ("distance = " + distance + ", radius = " + radius);
////						boidsRigidBody [j].velocity += (boidsRigidBody [i].velocity - boidsRigidBody [j].velocity) * Time.deltaTime;
//						if (distance < radius)
//						{
//							float avoidX = boids[i].transform.position.x - boids[j].transform.position.x;
//							float avoidY = boids [i].transform.position.y - boids [j].transform.position.y;
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