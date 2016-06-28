using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class boidsController : MonoBehaviour
{
	public float maxVelocity = 20;
	public float maxDist = 5000f;
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
					new Vector2 ((flockVelocity.x + 1.0f) * 100 * Mathf.Cos ((i * (360 / 5)) * Mathf.Deg2Rad),
						(flockVelocity.y + 1.0f) * 100 * Mathf.Sin ((i * (360 / 5)) * Mathf.Deg2Rad)));
				RaycastHit2D[] hits = Physics2D.RaycastAll (flockCenter, 
					new Vector2 ((flockVelocity.x + 1.0f) * 100 * Mathf.Cos ((i * (360 / 5)) * Mathf.Deg2Rad),
						(flockVelocity.y + 1.0f) * 100 * Mathf.Sin ((i * (360 / 5) + 360) * Mathf.Deg2Rad)));
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
			flockVelocity += new Vector2 (flockVelocity.x + Mathf.Cos ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad),
							flockVelocity.y + Mathf.Sin ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad));
			//		yield return null;
			Vector2 theCenter = Vector2.zero;
			int index = 0;
			//		Vector2 theVelocity = Vector2.zero;

			for (int i = 0; i < boids.Length; i++)
			{
				if (boids [i].activeSelf && boidsRigidBody[i].simulated && Vector2.Distance(theCenter, new Vector2(boids [i].transform.position.x, boids [i].transform.position.y)) < maxDist / 2.0f) 
				{
					index++;
					theCenter = theCenter + new Vector2 (boids [i].transform.position.x, boids [i].transform.position.y);
					//				theVelocity = theVelocity + boidsRigidBody [i].velocity.normalized;
				}
			}

			flockCenter = theCenter/(index);
			yield return new WaitForSeconds (2);
		}
	}

	void Update ()
	{
		counter.text = "Alive : " + flockSize + "/" + maxFlockSize;
		Vector2 theCenter = Vector2.zero;
		int index = 0;
		//		Vector2 theVelocity = Vector2.zero;

		for (int i = 0; i < boids.Length; i++)
		{
			if (boids [i].activeSelf && boidsRigidBody[i].simulated && Vector2.Distance(theCenter, new Vector2(boids [i].transform.position.x, boids [i].transform.position.y)) < maxDist / 2.0f) 
			{
				index++;
				theCenter = theCenter + new Vector2 (boids [i].transform.position.x, boids [i].transform.position.y);
				//				theVelocity = theVelocity + boidsRigidBody [i].velocity.normalized;
			}
		}

		flockCenter = theCenter/(index);
		//		flockVelocity = theVelocity/(flockSize);
		for (int i = 0; i < boids.Length; i++)
		{
			for (int j = 0; j < boids.Length; j++) {
				if (!boidsFlocking [i].alive && boidsFlocking[i].isActiveAndEnabled) 
				{
					flockSize--;
					boidsFlocking [i].gameObject.SetActive (false);
				}
			}
		}
	}
}