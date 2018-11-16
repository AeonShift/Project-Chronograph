using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZone : MonoBehaviour
{

    public TimeManager timeManager;

    public float timeSpeed;

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MovingPlatform":
                other.GetComponentInParent<PlatformMovement>().UpdateScale(timeSpeed);
                break;
            case "AirEnemy":
                other.GetComponent<FollowMovement>().UpdateScale(timeSpeed);
                break;
            case "GroundEnemy":
                other.GetComponent<EnemyGroundMovement>().UpdateScale(timeSpeed);
                break;
            case "Player":
                other.GetComponent<RaycastPlayerController>().UpdateScale(timeSpeed);
                break;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MovingPlatform":
                other.gameObject.GetComponentInParent<PlatformMovement>().UpdateScale(1.0f);
                break;
            case "AirEnemy":
                other.GetComponent<FollowMovement>().UpdateScale(1.0f);
                break;
            case "GroundEnemy":
                other.GetComponent<EnemyGroundMovement>().UpdateScale(1.0f);
                break;
            case "Player":
                other.GetComponent<RaycastPlayerController>().UpdateScale(1.0f);
                break;
        }
    }
}
