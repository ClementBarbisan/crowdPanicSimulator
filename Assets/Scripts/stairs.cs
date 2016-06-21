using UnityEngine;
using System.Collections;

public class stairs : MonoBehaviour {
	public GameObject arrow;
	public directionalArea direction;
	public float speed;

	IEnumerator shakeArrow()
	{
		while (true) 
		{
			arrow.transform.Translate (Vector2.down);
			yield return new WaitForSeconds (0.75f);
			arrow.transform.Translate (Vector2.up);
			yield return new WaitForSeconds (0.75f);
		}
	}
	// Use this for initialization
	void Start () {
		direction.speedX = Mathf.Cos ((transform.localRotation.eulerAngles.z - 90) * Mathf.Deg2Rad) * speed;
		direction.speedY = Mathf.Sin ((transform.localRotation.eulerAngles.z - 90) * Mathf.Deg2Rad) * speed;
		StartCoroutine ("shakeArrow");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		arrow.transform.localScale = new Vector3 (arrow.transform.localScale.x, -arrow.transform.localScale.y, arrow.transform.localScale.z);
		direction.speedX = -direction.speedX;
		direction.speedY = -direction.speedY;
	}
}
