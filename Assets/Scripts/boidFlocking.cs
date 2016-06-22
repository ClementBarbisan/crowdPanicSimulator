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
	private float maxAcceleration;
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
	private boidsController boidController;

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
		render.color = Color.white;
	}

	IEnumerator BoidSteering ()
	{
		while (true)
		{
			Vector2 direction = new Vector2(0.0f, 0.0f);
			if (inited)
			{
				foreach (attractor attract in attracts) {
					if (Mathf.Abs(attract.transform.localPosition.x - transform.localPosition.x) > 0.001f && Mathf.Abs(attract.transform.localPosition.y - transform.localPosition.y) > 0.001f)
						direction += new Vector2( attract.force / Mathf.Pow((transform.localPosition.x - attract.transform.localPosition.x), 2),  attract.force / Mathf.Pow((transform.localPosition.y - attract.transform.localPosition.y), 2));
				}
//				Debug.Log (direction.x);
				Vector2 tmp = (direction + dispatch +  Calc()) * Time.deltaTime;
				if (tmp.magnitude > maxAcceleration)
					rigidbody.velocity += rigidbody.velocity.normalized * maxAcceleration;
				else
					rigidbody.velocity += tmp;
//				rigidbody.velocity += dispatch + direction * Time.deltaTime;
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

	private Vector2 Calc ()
	{
		Vector2 flockCenter = boidController.flockCenter;
		Vector2 flockVelocity = boidController.flockVelocity;

		flockCenter = flockCenter - new Vector2(transform.localPosition.x, transform.localPosition.y);
		flockVelocity = flockVelocity - rigidbody.velocity;

		return (flockCenter + flockVelocity);
	}

	public void SetController (GameObject theController)
	{
		Controller = theController;
		boidController = Controller.GetComponent<boidsController>();
		minVelocity = boidController.minVelocity;
		maxVelocity = boidController.maxVelocity;
		randomness = boidController.randomness;
		maxAcceleration = boidController.maxAcceleration;
		inited = true;
	}

	void Update()
	{
		if (rigidbody.velocity.magnitude > maxAcceleration)
			rigidbody.velocity = rigidbody.velocity.normalized * maxAcceleration;
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
		if (coll.collider.tag == "wall" && ((touchAgent && (rigidbody.velocity.x > 12.0f || rigidbody.velocity.y > 12.0f)) || (rigidbody.velocity.x > 25.0f || rigidbody.velocity.y > 25.0f)))
			StartCoroutine ("die");
		if (coll.collider.tag == "wall")
		{
			rigidbody.velocity = Vector2.zero;
			stuckIndex += 2;
			touchWall = true;
		}
		if (coll.collider.tag == "agents") {
			rigidbody.velocity = Vector2.zero;
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
