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

    RaycastPlayerController player;

    /*Okay, idea time: Get the time functions in the playercontroller to return true while they're active, and if they're true
    while a timezone scaling is true, then use Time.DeltaTime! or Time.customDeltaTime depending on what is being used. Use
    customDeltaTime when freezing, but for fast and slow, they cancel, so just use Time.deltaTime*/
    //BIG QUESTION. IF TWO GAME OBJECTS ARE USING THE SAME SCRIPT, CAN YOU CHANGE A VARIABLE IN ONE OF THEM AND KEEP IT THE SAME IN THE OTHER ONE????
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
