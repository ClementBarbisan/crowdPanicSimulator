using UnityEngine;
using System.Collections;

public class arrival : MonoBehaviour {
	private int count;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "agents")
			count++;
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.tag == "agents") 
		{
			Rigidbody2D tmp = coll.GetComponent<Rigidbody2D> ();
			tmp.simulated = false;
		}
	}
}
