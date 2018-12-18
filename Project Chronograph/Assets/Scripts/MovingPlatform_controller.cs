using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement2D))]
public class MovingPlatform_controller : MonoBehaviour {
    //MAKE SURE TO SET THE PASSENGERMASK TO WHATEVER YOU WANNA MOVE 13:51
    public Vector3 move;

    Movement2D Movement;

	// Use this for initialization
	public void Start () {
        Movement = GetComponent<Movement2D>();
    }
	
	// Update is called once per frame
	void Update () {
        Movement.UpdateRaycastOrigins();
        Movement.MovePassengers(move);
        Movement.MovePlatform(move);
	}
}
