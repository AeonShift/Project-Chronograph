using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

    public int damageToGive;
    private Rigidbody2D rb2d;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update () {

	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<RaycastPlayerController>();
        if (!HealthManager.invincible) { 
            if (other.CompareTag("Player"))
            {
                HealthManager.invincible = true;
                float direction = GetSign(rb2d.velocity.x);
                HealthManager.HurtPlayer(damageToGive);
                //from left to right, enemy movement direction, knockback duration, and knockbackspeed
                player.KnockbackFunc(direction, .08f, 5);
                Invoke("resetInvulnerability", 1);
            }
        }
    }

    //helper function for finding our wanted and velocity directions
    private int GetSign(float input)
    {
        if (Mathf.Approximately(input, 0))
        {
            return 0;
        }
        else if (input > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private void resetInvulnerability(){
        HealthManager.invincible = false;
    }
}
