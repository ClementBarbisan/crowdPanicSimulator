using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class arrival : MonoBehaviour {
	public boidsController manager;
	public Text winText;
	private int flockMax;
	private int count;

	// Use this for initialization
	void Start () {
		flockMax = manager.flockSize;
		winText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if (count >= manager.flockSize)
			winText.text = "Victory\n" + count + "/" + flockMax;
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
