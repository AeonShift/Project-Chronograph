using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {

    private bool playerEnd;
    public string LoadLevel;

	// Use this for initialization
	void Start () {
        playerEnd = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(playerEnd)
        {
            Application.LoadLevel(LoadLevel); 
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if(other.name == "Player")
        {
            playerEnd = true;
        }
    }

    }


