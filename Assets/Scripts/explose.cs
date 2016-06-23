using UnityEngine;
using System.Collections;

public class explose : MonoBehaviour {
	public AudioSource explosionSound;
	// Use this for initialization
	void Start () {
	
	}

	public void explosion()
	{
		explosionSound.Play ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
