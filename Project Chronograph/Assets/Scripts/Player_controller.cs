using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this makes sure that the Player has the Movement2D script attached so it doesn't start without it
[RequireComponent (typeof (Movement2D))] //THIS ALSO AUTOMATICALLY ADDS THE SCRIPTS NEED WOOOOOO STREAMLINING!!!
public class Player_controller : MonoBehaviour {

    //these two variables give a more intuitive way to assing gravity and jump velocity rather than changing them directly
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    public float moveSpeed = 6;
    public float gravity;
    public float jumpVelocity;
    Vector3 velocity;
    Movement2D Movement;

	// Use this for initialization
	void Start () {
        Movement = GetComponent<Movement2D>();
	}
	
	// Update is called once per frame
	void Update () {

        //if you're hitting something above you or below you, velocity will change to zero.
        if (Movement.collisions.above || Movement.collisions.below) {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButton("Jump") && Movement.collisions.below) {
            velocity.y = jumpVelocity;

        }

        velocity.x = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        Movement.Move(velocity * Time.deltaTime);
    }
}
