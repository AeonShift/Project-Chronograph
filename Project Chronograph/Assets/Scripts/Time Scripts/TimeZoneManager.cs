using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZoneManager : MonoBehaviour {

    public float scalingFactor = 1f;
    public float zoneFactor = 1f;
    public float slowFactor = 0.05f;
    public float fastFactor = 2;
    public float slowCustomDeltaTime;

    private void Update()
    {
        slowCustomDeltaTime = Time.deltaTime * slowFactor;
    }


    public void SlowTimeZone(float timeVariable)
    {
        timeVariable = slowCustomDeltaTime;
    }

    public void SlowScaleZone(float timeVariable)
    {
        timeVariable = slowFactor;
    }

}
