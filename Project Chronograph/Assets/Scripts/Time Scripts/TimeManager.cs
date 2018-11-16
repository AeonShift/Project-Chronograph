using UnityEngine;

public class TimeManager : MonoBehaviour {

    public float scalingFactor = 1f;
    public float zoneFactor = 1f;
    public float slowFactor = 0.05f;
    public float fastFactor = 2;
    public float customDeltaTimePlatforms;
    public float customDeltaTimeEnemies;
    public float customDeltaTimePlayer;
    public float customDeltaTime;
    public float slowTimeZoneDeltaTime;
    public float speedTimeZoneDeltaTime;
    public float timeZoneScalingFactor;

    //make another time manager specifically for time zones and then make custom delta times insdie of each script and then change
    //the customdeltatime in each script with the time zone

    private void Update()
    {
        customDeltaTime = Time.deltaTime * scalingFactor;
        slowTimeZoneDeltaTime = Time.deltaTime * slowFactor;
        speedTimeZoneDeltaTime = Time.deltaTime * fastFactor;
    }

    public bool Slowdown() {

        scalingFactor = slowFactor;
        return true;  
    }

    public void Speedup() {

        scalingFactor = fastFactor; 
    }

    public void Freeze() {

        scalingFactor = 0;

    }
    public void UndoTime() {

        scalingFactor = 1f;

    }
    public void SlowZoneEffect(float zoneSpeed)
    {
        if(player.isSlowUsed){
            customDeltaTime = customDeltaTime;
        }
        else if(player.isSpeedUsed){
            customDeltaTime = Time.deltaTime;
        }

    }



	
}
