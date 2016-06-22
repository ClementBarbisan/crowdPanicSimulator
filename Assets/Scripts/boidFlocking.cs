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
	private attractor currentAttract;
	public Vector2 dispatch;
	private Vector2 oldDirection;
	private bool touchAgent = false;
	private bool touchWall = false;
	private int stuckIndex = 0;

	void Awake()
	{
		attracts = FindObjectsOfType<attractor>();
		render = GetComponent<SpriteRenderer> ();
		currentAttract = GetComponent<attractor> ();
	}

	void Start ()
	{
		StartCoroutine ("BoidSteering");
		dispatch = new Vector2 (0.0f, 0.0f);
		oldDirection = new Vector2 (0.0f, 0.0f);
		render.color = Color.green;
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
				rigidbody.velocity += dispatch + (direction - oldDirection);
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
			render.color = Color.red;
		if (stuckIndex > 4)
			StartCoroutine ("stuck");
	}

	IEnumerator stuck()
	{
		int i = 0;
		while (((stuckIndex > 3 && touchWall) || stuckIndex > 4) && stuckIndex < 7) 
		{
			i++;
			if (i > 7000 - stuckIndex * 1000)
				StartCoroutine ("die");
			yield return null;
		}
	}

	IEnumerator die()
	{
		stun = true;
		GetComponent<CircleCollider2D> ().enabled = false;
		rigidbody.simulated = false;
		yield return new WaitForSeconds(2);
		alive = false;
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.collider.tag == "agents") {
			stuckIndex--;
			touchAgent = false;
		}
		if (coll.collider.tag == "wall") {
			stuckIndex -= 2;
			touchWall = false;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.collider.tag == "wall" && ((touchAgent && (rigidbody.velocity.x > 12.0f || rigidbody.velocity.y > 12.0f)) || (rigidbody.velocity.x > 20.0f || rigidbody.velocity.y > 20.0f)))
			StartCoroutine ("die");
		if (coll.collider.tag == "wall")
		{
			stuckIndex += 2;
			touchWall = true;
		}
		if (coll.collider.tag == "agents") {
			stuckIndex++;
			touchAgent = true;
		}
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown (0)) {
			currentAttract.force += 3;
			render.color += new Color (0.1f, 0.1f, 0.1f, 0.0f);
		} else if (Input.GetMouseButtonDown (1)) {
			currentAttract.force -= 3;
			render.color -= new Color (0.1f, 0.1f, 0.1f, 0.0f);
		}
	}
}
