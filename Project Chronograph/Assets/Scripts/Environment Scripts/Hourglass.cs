using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourglass : MonoBehaviour
{
    public bool playerTimeZone;
    public float startingTime;
    private float countingTime;
    public bool countdown;
    public TimeManager timeManager;

    // Use this for initialization
    void Start()
    {
        countingTime = startingTime;
        playerTimeZone = false;
        countdown = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) && playerTimeZone)
        {
            countdown = true;
        }
        
        if (Input.GetKey(KeyCode.T) && playerTimeZone && countdown)
        {
            countdown = false;
        }
        if(countdown)
        {
            countingTime -= timeManager.customDeltaTime;
        }

        if (countingTime <= 0)
        {
            HealthManager.KillPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            playerTimeZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            playerTimeZone = false;
        }
    }
    public void ResetTime()
    {
        countingTime = startingTime;
    }

}