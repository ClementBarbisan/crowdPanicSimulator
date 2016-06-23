using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class arrival : MonoBehaviour {
	public boidsController manager;
	public Text winText;
	public Text arrivedText;
	public Text timer;
	public int time;
	private int flockMax;
	private int count;
    public string scene;
	private bool end = false;

	// Use this for initialization
	void Start () {
		flockMax = manager.flockSize;
		winText.text = "";
		arrivedText.text = "";
		timer.text = "00:" + time;
		StartCoroutine ("startTimer");
	}

	IEnumerator startTimer()
	{
		while (time >= 0 && !end)
		{
			yield return new WaitForSeconds (1);
			if (time >= 10)
				timer.text = "00:" + time;
			else
				timer.text = "00:0" + time;
			if (time == 0)
				StartCoroutine ("loadScene");
			time--;
		}
	}

    IEnumerator loadScene()
    {
		end = true;
		winText.text = count + "/" + flockMax;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(scene);
    }

	// Update is called once per frame
	void Update () {
		if (!end)
			arrivedText.text = "Safe : " + count + "/" + flockMax;
//        if (count >= manager.flockSize)
//        {
//            winText.text = count + "/" + flockMax;
//            StartCoroutine ("loadScene");
//        }
		if (Input.GetKeyDown (KeyCode.R))
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
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
			coll.gameObject.SetActive (false);
		}
	}
}
