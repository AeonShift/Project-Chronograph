using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    [SerializeField]
    private Vector2 dir;
    [SerializeField]
    private float radius;
    [SerializeField]
    private float speed;

    private Vector2 center;
    private float timer;

    public TimeManager timeManager;



    // Use this for initialization
    void Start () {
        center = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        timer += timeManager.customDeltaTime;
        transform.position = center + dir * Mathf.Sin(timer * speed) * radius;
	}
}
