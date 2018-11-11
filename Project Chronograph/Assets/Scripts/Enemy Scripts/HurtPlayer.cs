using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

    public int damageToGive;


	// Use this for initialization
	void Start () {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<RaycastPlayer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<RaycastPlayerController>();
        if (other.CompareTag("Player"))
        {
            HealthManager.HurtPlayer(damageToGive);
            //from left to right, knockback duration, knockbackspeed, and position to move from
            player.KnockbackFunc(0.08f, 5, player.transform.position);
        }
        
    }
}
