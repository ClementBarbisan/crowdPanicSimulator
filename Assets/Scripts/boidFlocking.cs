using UnityEngine;
using System.Collections;

public class boidFlocking : MonoBehaviour
{
	private GameObject Controller;
	private float distMax = 5000f;
	private bool inited = false;
	public bool alive = true;
	public bool stun = false;
	private float maxVelocity;
	private float maxAcceleration;
	public Rigidbody2D rigidbody;
	private SpriteRenderer render;
	public attractor[] attracts;
	private attractor currentAttract;
	public Vector2 dispatch;
	private Vector2 currentCenter;
	private bool touchAgent = false;
	private bool touchWall = false;
	private int stuckIndex = 0;
	public boidsController boidController;
	public AudioSource soundying;
	private Animator anim;
	private Vector2 currentWallPosition;
	public SpriteRenderer circleRange;

	void Awake()
	{
		render = GetComponent<SpriteRenderer> ();
		currentAttract = GetComponent<attractor> ();
		anim = GetComponent<Animator> ();
	}

	IEnumerator raycastSearch()
	{
		while (true) {
			int maxIndex = 0;
			float maxValue = 0;
			for (int i = 0; i < 5; i++) {
				Debug.DrawRay (transform.position, 
					new Vector2 (rigidbody.velocity.x * 20 * Mathf.Cos ((i * (180 / 5) + 180) * Mathf.Deg2Rad),
						rigidbody.velocity.y * 20 * Mathf.Sin ((i * (180 / 5) + 180) * Mathf.Deg2Rad)));
				RaycastHit2D[] hits = Physics2D.RaycastAll (transform.position, 
					                     new Vector2 (rigidbody.velocity.x * 20 * Mathf.Cos ((i * (180 / 5) + 180) * Mathf.Deg2Rad),
						                     rigidbody.velocity.y * 20 * Mathf.Sin ((i * (180 / 5) + 180) * Mathf.Deg2Rad)));
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
			boidController.flockCenter += new Vector2 (rigidbody.velocity.x + Mathf.Cos ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad),
				rigidbody.velocity.y + Mathf.Sin ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad)) * boidController.radius;
//			rigidbody.velocity += new Vector2 (rigidbody.velocity.x + Mathf.Cos ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad),
//				rigidbody.velocity.y + Mathf.Sin ((maxIndex * (180 / 5) + 180) * Mathf.Deg2Rad));
//		yield return null;
			yield return new WaitForSeconds (2);
		}
	}

	void Start ()
	{
		circleRange.gameObject.SetActive (false);
		transform.rotation = Quaternion.Euler (new Vector3(0.0f, 0.0f, Mathf.PI / 2.0f));
		anim.SetBool ("stopped", false);
//		StartCoroutine ("raycastSearch");
		dispatch = new Vector2 (0.0f, 0.0f);
		attracts = FindObjectsOfType<attractor>();
		render.color = Color.white;
	}

	private Vector2 Calc ()
	{
		Vector2 flockCenter = boidController.flockCenter;
		Vector2 flockVelocity = boidController.flockVelocity;

		flockCenter = flockCenter - new Vector2(transform.position.x, transform.position.y);
//		flockVelocity = flockVelocity - rigidbody.velocity;

		return (flockVelocity + flockCenter);
	}

	public void SetController (GameObject theController)
	{
		Controller = theController;
		boidController = Controller.GetComponent<boidsController>();
		maxVelocity = boidController.maxVelocity;
		maxAcceleration = boidController.maxAcceleration;
		distMax = boidController.maxDist;
		inited = true;
	}

	void Update()
	{
//		if (rigidbody.velocity.magnitude > maxAcceleration)
//			rigidbody.velocity = rigidbody.velocity.normalized * maxAcceleration;
		transform.Rotate(new Vector3(0.0f, 0.0f, Mathf.Atan2(transform.position.y, transform.position.x) - transform.rotation.z));
		if (stun)
			render.color = new Color(1.0f, 1.0f - 1.0f / 12.0f * stuckIndex, 1.0f - 1.0f / 12.0f * stuckIndex, 1.0f);
		else
			render.color = Color.white;
		if (stuckIndex > 4)
			StartCoroutine ("stuck");
		if (inited)
		{
			Vector2 direction = new Vector2(0.0f, 0.0f);
			Vector3 coordinates = Camera.main.WorldToScreenPoint (transform.position);
			foreach (attractor attract in attracts) {
				Vector3 coordinatesAttract = Camera.main.WorldToScreenPoint (attract.transform.position);
				float dist = Vector3.Distance (coordinatesAttract, coordinates) * 20;
				if (dist > 0.001f && dist < distMax)
					direction += new Vector2( (attract.force / (dist)) * (coordinatesAttract.x - coordinates.x),  (attract.force / (dist)) * (coordinatesAttract.y - coordinates.y));
			}
//			direction /= attracts.Length;
//			flockVelocity += direction * Time.deltaTime;
			currentCenter = Calc();
			Vector2 tmp = (dispatch + currentCenter + direction) * Time.deltaTime;
			if (touchWall)
				tmp = (dispatch + currentCenter) * Time.deltaTime;

			//				Debug.Log ("Vector.x = " + tmp.x + ", vector.y = " + tmp.y);
			if (tmp.magnitude > maxAcceleration)
				rigidbody.velocity += tmp.normalized * maxAcceleration;
			else
				rigidbody.velocity += tmp;
			//				rigidbody.velocity += dispatch + direction * Time.deltaTime;
			// enforce minimum and maximum speeds for the boids
			float speed = rigidbody.velocity.magnitude;
			if (speed > maxVelocity) {
				rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
			} 
		}
	}

	IEnumerator stuck()
	{
		int i = 0;
		stun = true;
		while (((stuckIndex > 3 && touchWall) || stuckIndex > 4) && stuckIndex < 12) 
		{
			i++;
			if (i > 11050 - stuckIndex * 1000) {
				StartCoroutine ("die");
				yield break;
			}
			yield return null;
		}
		stun = false;
	}

	IEnumerator die()
	{
		GetComponent<CircleCollider2D> ().enabled = false;
		rigidbody.simulated = false;
		GetComponent<SpriteRenderer> ().sortingOrder = 3;
		transform.localScale *= 0.2f;
		anim.SetBool ("stopped", true);
		soundying.Play ();
		yield return new WaitForSeconds(5);
		alive = false;
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.tag == "agents") {
			rigidbody.velocity = rigidbody.velocity.normalized;
			stuckIndex--;
			touchAgent = false;
		}
		else if (coll.tag == "wall") {
//			rigidbody.velocity = rigidbody.velocity.normalized;
//			boidController.flockVelocity = rigidbody.velocity.normalized;
			stuckIndex -= 2;
			touchWall = false;
		}
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.tag == "wall") {
			//			boidController.flockVelocity = -rigidbody.velocity.normalized;
//			boidController.flockCenter -= rigidbody.velocity;
			float avoidX = transform.position.x - coll.transform.position.x;
			float avoidY = transform.position.y - coll.transform.position.y;
			float angle = Mathf.Atan2 (avoidY, avoidX);
			rigidbody.velocity = new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius);
//			boidController.flockCenter += new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius) * Time.deltaTime;
			boidController.flockVelocity = new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius) * Time.deltaTime;
		}
		else if (coll.tag == "agents") {
			if (touchWall) {
				float avoidX = transform.position.x - currentWallPosition.x;
				float avoidY = transform.position.y - currentWallPosition.y;
				float angle = Mathf.Atan2 (avoidY, avoidX);
				rigidbody.velocity = new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius);
				if (Vector2.Distance (transform.position, coll.transform.position) < boidController.radius / 3.0f)
					stuckIndex++;
			}
			else
			{
				float avoidX = transform.position.x - coll.transform.position.x;
				float avoidY = transform.position.y - coll.transform.position.y;
				float angle = Mathf.Atan2 (avoidY, avoidX);
				rigidbody.velocity += new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "wall" && ((touchAgent && (rigidbody.velocity.x > 12.0f || rigidbody.velocity.y > 12.0f)) || (rigidbody.velocity.x > 25.0f || rigidbody.velocity.y > 25.0f)))
			StartCoroutine ("die");
		if (coll.tag == "wall")
		{
			float avoidX = transform.position.x - coll.transform.position.x;
			float avoidY = transform.position.y - coll.transform.position.y;
			float angle = Mathf.Atan2 (avoidY, avoidX);
			boidController.flockCenter += new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius);
			boidController.flockVelocity = new Vector2 (Mathf.Cos (angle) * boidController.radius, Mathf.Sin (angle) * boidController.radius);
//			boidController.flockVelocity = Vector2.zero;
			stuckIndex += 2;
			touchWall = true;
			rigidbody.velocity = Vector2.zero;
			currentWallPosition = coll.transform.position;
		}
		if (coll.tag == "agents") {
			stuckIndex++;
			touchAgent = true;
		}

	}
	void OnMouseEnter()
	{
		circleRange.gameObject.SetActive (true);
	}

	void OnMouseExit()
	{
		circleRange.gameObject.SetActive (false);
	}

	void OnMouseOver()
	{
		circleRange.gameObject.SetActive (true);
		if (Input.GetMouseButtonDown (1) && currentAttract.force < 90) {
			currentAttract.force += 30;
			render.color += new Color (0.0f, 0.25f, 0.0f, 0.0f);
		} else if (Input.GetMouseButtonDown (0) && currentAttract.force > -100) {
			currentAttract.force -= 50;
			boidController.flockSize--;
			rigidbody.simulated = false;
			anim.speed = 0;
			render.color -= new Color (0.0f, 0.0f, 0.5f, 0.0f);
		}
	}
}
