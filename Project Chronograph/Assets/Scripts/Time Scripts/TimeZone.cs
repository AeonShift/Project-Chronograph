using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZone : MonoBehaviour
{

    public TimeManager timeManager;

    public float timeSpeed;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //timeManager.SlowZoneEffect(timeSpeed);
        }

    }

    void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                timeManager.UndoTime();
            }


        }

    }