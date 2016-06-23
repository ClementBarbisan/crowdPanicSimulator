using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {


    
	public void LoadScene(string name)
    {
        Application.LoadLevel(name);  
    }
}
