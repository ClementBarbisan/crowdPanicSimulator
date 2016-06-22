using UnityEngine;
using System.Collections;

public class directionalArea : MonoBehaviour {
	public float speedX = 1;
	public float speedY = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		boidFlocking tmp;
		if ((tmp = coll.GetComponent<boidFlocking> ()))
			tmp.rigidbody.velocity += new Vector2 (speedX * Time.deltaTime, speedY * Time.deltaTime);
	}
}
