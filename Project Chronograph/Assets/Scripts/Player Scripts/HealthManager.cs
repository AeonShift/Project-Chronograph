using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public int maxPlayerHealth;

    public static int playerHealth;

    public static bool invincible = false;

    //To display health through the heart images
    Image hearts;
    public Sprite heartsFull;
    public Sprite heartsHalf;
    public Sprite heartsOne;
    public Sprite heartsNone;

    Text healthText;

    private LevelManager levelManager;

    private Hourglass hourglass;

	// Use this for initialization
	void Start () {
        healthText = GetComponent<Text>();
        playerHealth = maxPlayerHealth;
        hearts = GetComponent<Image>();
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
        //healthText.text = "Health: " + playerHealth;
        if(playerHealth == 3){
            hearts.sprite = heartsFull;
        }
        else if(playerHealth == 2){
            hearts.sprite = heartsHalf;
        }
        else if(playerHealth == 1){
            hearts.sprite = heartsOne;
        }
        else if(playerHealth <= 0){
            hearts.sprite = heartsNone;
        }
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
