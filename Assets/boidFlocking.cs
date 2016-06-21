using UnityEngine;
using System.Collections;

public class boidFlocking : MonoBehaviour
{
	private GameObject Controller;
	private bool inited = false;
	private float minVelocity;
	private float maxVelocity;
	private float randomness;
	private GameObject chasee;
	public Rigidbody2D rigidbody;
	public attractor[] attracts;
	public Vector2 dispatch;
	private Vector2 oldDirection;

	void Awake()
	{
		GameObject[] tmp = GameObject.FindGameObjectsWithTag ("attractor");
		attracts = new attractor[tmp.Length];
		for (int i = 0; i < tmp.Length; i++)
			attracts[i] = tmp[i].GetComponent<attractor>();
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	void Start ()
	{
		StartCoroutine ("BoidSteering");
		dispatch = new Vector2 (0.0f, 0.0f);
		oldDirection = new Vector2 (0.0f, 0.0f);
	}

	IEnumerator BoidSteering ()
	{
		while (true)
		{
			Vector2 direction = new Vector2(0.0f, 0.0f);
			if (inited)
			{
				foreach (attractor attract in attracts) {
					if (20.0f - Vector2.Distance (transform.position, attract.transform.position) > 0.0f)
						direction += new Vector2(attract.force * (20.0f - Vector2.Distance (transform.position.normalized, attract.transform.position.normalized)) / 20.0f, attract.force * (20.0f - Vector2.Distance (transform.position.normalized, attract.transform.position.normalized)) / 20.0f);
				}
//				Debug.Log ("direction = " + (direction - oldDirection));
				rigidbody.velocity += dispatch + (direction - oldDirection);
				// enforce minimum and maximum speeds for the boids
				float speed = rigidbody.velocity.magnitude;
				if (speed > maxVelocity) {
					rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
				} 
				else if (speed < minVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
				}
			}

//			float waitTime = Random.Range(0.3f, 0.5f);
//			yield return new WaitForSeconds (waitTime);
			oldDirection = direction;
			yield return null;
		}
	}
		
	public void SetController (GameObject theController)
	{
		Controller = theController;
		boidsController boidController = Controller.GetComponent<boidsController>();
		minVelocity = boidController.minVelocity;
		maxVelocity = boidController.maxVelocity;
		randomness = boidController.randomness;
		chasee = boidController.chasee;
		inited = true;
	}
}
