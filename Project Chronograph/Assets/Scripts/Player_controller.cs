using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this makes sure that the Player has the Movement2D script attached so it doesn't start without it
[RequireComponent (typeof (Movement2D))] //THIS ALSO AUTOMATICALLY ADDS THE SCRIPTS NEED WOOOOOO STREAMLINING!!!
public class Player_controller : MonoBehaviour {

    //these two variables give a more intuitive way to assing gravity and jump velocity rather than changing them directly
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float moveSpeed = 6;
    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;


    Movement2D Movement;

	// Use this for initialization
	void Start () {
        Movement = GetComponent<Movement2D>();
        //just some physics to set the gravity and jump velocity
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
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

        float targetVelocityx = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityx, ref velocityXSmoothing, Movement.collisions.below ? accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        Movement.MovePlayer(velocity * Time.deltaTime);
    }
}
