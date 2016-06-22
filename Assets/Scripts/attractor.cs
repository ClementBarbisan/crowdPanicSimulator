using UnityEngine;
using System.Collections;

public class attractor : MonoBehaviour {
	public float force;
	private float initForce;
	// Use this for initialization
	void Start () {
		initForce = force;
	}

	public void reset()
	{
		force = initForce;
	}

	public void explode()
	{
		force -= 50;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			force -= 1000;
	}
}
