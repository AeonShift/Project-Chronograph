using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//this makes sure that the Player has the Movement2D script attached so it doesn't start without it
[RequireComponent (typeof (Movement2D))] //THIS ALSO AUTOMATICALLY ADDS THE SCRIPTS NEED WOOOOOO STREAMLINING!!!
public class Player_controller : MonoBehaviour {

    //these two variables give a more intuitive way to assing gravity and jump velocity rather than changing them directly
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float moveSpeed = 6;

    public float wallSlideSpeedMax = 3;
    public Vector2 wallJumpStrong = new Vector2(25,18);
    public Vector2 wallJumpWeak = new Vector2(15,10);
    public float wallStickTime = .25f;
    float timeToWallUnstick;


    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;


    Movement2D Movement;

	// Use this for initialization
	void Start () {
        Movement = GetComponent<Movement2D>();
        //just some physics to set the gravity and jump velocity
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        Movement.collisions.faceDir = 1;
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (Movement.collisions.left) ? -1 : 1;

        float targetVelocityx = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityx, ref velocityXSmoothing, Movement.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;
        Movement.WallSliding(input, ref velocity, ref velocityXSmoothing, wallSlideSpeedMax, ref wallSliding, wallStickTime, timeToWallUnstick, wallDirX);




        if (Input.GetButtonDown("Jump")) {
            Movement.JumpPlayer(ref velocity, maxJumpVelocity, minJumpVelocity, wallSliding, wallDirX, wallJumpStrong, wallJumpWeak, input);
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }



        velocity.y += gravity * Time.deltaTime;
        Movement.MovePlayer(velocity * Time.deltaTime);
    }
}
