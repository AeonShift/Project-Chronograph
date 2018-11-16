﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

    public GameObject platform;

    public float moveSpeed;

    private Transform currentPoint;

    public Transform[] points;

    public TimeManager timeManager;

    public int pointSelection;

    private float timeScalingFactor = 1.0f;

    // Use this for initialization
    void Start () {

        currentPoint = points[pointSelection];
		
	}
	
	// Update is called once per frame
	void Update () {

        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentPoint.position, timeManager.customDeltaTime * moveSpeed * timeScalingFactor);
       
        if (platform.transform.position == currentPoint.position)
        {

            pointSelection++;

            if(pointSelection == points.Length) 
            {
                pointSelection = 0;
            }

            currentPoint = points[pointSelection];

        }
	}

    public void UpdateScale(float scale)
    {
        timeScalingFactor = scale;
    }

}
