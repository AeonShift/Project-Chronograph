using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

    public int damageToGive;
    private RaycastPlayerController player;

	// Use this for initialization
	void Start () {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<RaycastPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            HealthManager.HurtPlayer(damageToGive);

            //from left to right, knockback duration, knockbackspeed, and position to move from
            StartCoroutine(player.Knockback(0.02f, 350, player.transform.position));
        }
        
    }
}
