using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZone : MonoBehaviour {

    public TimeManager timeManager;

    public float timeSpeed;
    public bool isAffected = false;

    private List<GameObject> affectedObjects = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "MovingPlatform" || other.tag == "AirEnemy" || other.tag == "GroundEnemy"|| other.tag == "Player")
        {
            Debug.Log("affected");
            affectedObjects.Add(other.gameObject);
            isAffected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "MovingPlatform" || other.tag == "Enemy" || other.tag == "Player")
        {
            Debug.Log("Defected");
            affectedObjects.Remove(affectedObjects.Find(x => x.name.Equals(other.name)));
            isAffected = false;
        }
    }

    private void Update()
    {
        for(int i = 0; i < affectedObjects.Count; i++)
        {
            if(affectedObjects[i].tag == "Player"){
                affectedObjects[i].GetComponent<RaycastPlayerController>().SlowUpdateScale(.35f);
            }
            else if(affectedObjects[i].tag == "Enemy"){
                affectedObjects[i].GetComponent<EnemyGroundMovement>().SlowUpdateScale(.35f);
            }
            //affectedObjects[i].GetComponent<RaycastPlayerController>().velocity *= timeSpeed;
        } 
    }
}
