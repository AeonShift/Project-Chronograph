using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public string startLevel;
    public Text displayText;


    public void NewGame()
    {
        Application.LoadLevel(startLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

	// Use this for initialization
	void Start () {
   
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
