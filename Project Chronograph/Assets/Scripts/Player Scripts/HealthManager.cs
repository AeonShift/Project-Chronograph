using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public int maxPlayerHealth;

    public static int playerHealth;

    public static bool invincible = false;

   //GameObject player = GameObject.Find("RaycastPlayer");
    //Animation anim = player.GetComponent<Animation>();


    Text text;

    private LevelManager levelManager;

    private Hourglass hourglass;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        playerHealth = maxPlayerHealth;
        levelManager = FindObjectOfType<LevelManager>();
        hourglass = FindObjectOfType<Hourglass>();
    }
	
	// Update is called once per frame
	void Update () {

		if(playerHealth <= 0)
        {
            playerHealth = 0;
            levelManager.RespawnPlayer();
        }
        text.text = "" + playerHealth;
	}

    public static void HurtPlayer(int damage)
    {
        GameObject.Find("RaycastPlayer").GetComponent<Animation>().Play("Player_RedFlash");
        playerHealth -= damage;
    }


   

    public void FullHp()
    {
        playerHealth = maxPlayerHealth;
    }
    
    public static void KillPlayer()
    {
        playerHealth = 0;
    }
}
