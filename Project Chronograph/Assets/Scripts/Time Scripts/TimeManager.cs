using UnityEngine;

public class TimeManager : MonoBehaviour {

    public float scalingFactor = 1f;
    public float zoneFactor = 1f;
    public float slowFactor = 0.05f;
    public float fastFactor = 2;
    public float customDeltaTime;

    //make another time manager specifically for time zones and then make custom delta times insdie of each script and then change
    //the customdeltatime in each script with the time zone
    private void Update()
    {
        customDeltaTime = Time.deltaTime * scalingFactor;

    }

    public void Slowdown() {

        scalingFactor = slowFactor;
        
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



	
}
