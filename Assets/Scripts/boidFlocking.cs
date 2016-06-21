using UnityEngine;
using System.Collections;

public class boidFlocking : MonoBehaviour
{
	private GameObject Controller;
	private bool inited = false;
	public bool alive = true;
	public bool stun = false;
	private float minVelocity;
	private float maxVelocity;
	private float randomness;
	public Rigidbody2D rigidbody;
	private SpriteRenderer render;
	public attractor[] attracts;
	public Vector2 dispatch;
	private Vector2 oldDirection;
	private bool touchAgent = false;
	private bool touchWall = false;

	void Awake()
	{
		GameObject[] tmp = GameObject.FindGameObjectsWithTag ("attractor");
		attracts = new attractor[tmp.Length];
		for (int i = 0; i < tmp.Length; i++)
			attracts[i] = tmp[i].GetComponent<attractor>();
		rigidbody = GetComponent<Rigidbody2D> ();
		render = GetComponent<SpriteRenderer> ();
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
				rigidbody.velocity += dispatch + (direction - oldDirection * 0.01f);
				// enforce minimum and maximum speeds for the boids
				float speed = rigidbody.velocity.magnitude;
				if (speed > maxVelocity) {
					rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
				} 
//				else if (speed < minVelocity)
//				{
//					rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
//				}
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
		inited = true;
	}

	void Update()
	{
		if (stun)
			render.color = Color.blue;
		else if (alive)
			render.color = Color.green;
		else
			render.color = Color.red;
	}

	IEnumerator stuck()
	{
		int i = 0;
		while (touchAgent && touchWall && alive) {
			i++;
			yield return new WaitForSeconds(1);
			if (i >= 3)
				alive = false;
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.collider.tag == "agents")
			touchAgent = false;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (touchAgent && coll.collider.tag == "wall" && (rigidbody.velocity.x > 12.0f || rigidbody.velocity.y > 12.0f))
			alive = false;
		if (coll.collider.tag == "wall")
		{
			StartCoroutine ("stuck");
			touchWall = true;
		}
		if (coll.collider.tag == "agents") 
			touchAgent = true;
	}
}
