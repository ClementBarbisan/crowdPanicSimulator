using UnityEngine;
using System.Collections;

public class Open : MonoBehaviour {

    public Animator OpenTheDoor;
   

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

      

    }


    void OnMouseDown()
    {
        OpenTheDoor.SetBool("isOpen?", true);
    }

    
       
    
}
