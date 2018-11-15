using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZone : MonoBehaviour {

    public TimeManager timeManager;

    public float timeSpeed;

    private List<GameObject> affectedObjects = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "MovingPlatform" || other.tag == "Enemy" || other.tag == "Player")
        {
            Debug.Log("affected");
            affectedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "MovingPlatform" || other.tag == "Enemy" || other.tag == "Player")
        {
            Debug.Log("Defected");
            affectedObjects.Remove(affectedObjects.Find(x => x.name.Equals(other.name)));
        }
    }

    private void Update()
    {
        for(int i = 0; i < affectedObjects.Count; i++)
        {
           affectedObjects[i].GetComponent<Rigidbody2D>().velocity *= timeSpeed;
        } 
    }
}
