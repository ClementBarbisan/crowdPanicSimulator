using UnityEngine;
using System.Collections;

public class attractor : MonoBehaviour {
	public float force;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			force -= 1000;
	}
}
