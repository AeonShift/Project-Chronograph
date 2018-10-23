using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public GameObject currentCheckpoint;
    public float respawnDelay;
    public HealthManager healthManager;


    private PlayerController player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();
        healthManager = FindObjectOfType<HealthManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void RespawnPlayer()
    {
        StartCoroutine("RespawnPlayerCo");
    }

    public IEnumerator RespawnPlayerCo()
    {
        player.enabled = false;
        player.GetComponent<Renderer>().enabled = false;
        Debug.Log("Player Respawn");
        yield return new WaitForSeconds(respawnDelay);
        player.enabled = true;
        player.GetComponent<Renderer>().enabled = true;
        healthManager.FullHp();
        player.transform.position = currentCheckpoint.transform.position;
        
       
    }
}
