﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//You're at 1:29:07 in the video-
public class RaycastPlayerController : MonoBehaviour {

    //this enumerator stors the two states of jumping
    private enum JumpState{
        None = 0, Holding
    }

    //The SerializeFields are so that we can see the variables in the inspector even though they're private
    [SerializeField]
    private LayerMask platformMask;
    [SerializeField]
    private float gravity;
    //For accelerating the player
    [SerializeField]
    private float horizAccel;
    //for stopping the player so they don't slide everywhere
    [SerializeField]
    private float horizDeccel;
    //the max speed the player can go
    [SerializeField]
    private float horizMaxSpeed;
    /*for setting the velocity of the player as soon as they turn around so that they don't have to gradually 
     * go from max velocity to the right to 0 to max velocity to the left*/ 
    [SerializeField]
    private float horizSnapSpeed;
    //these lengths are for modifying the starting points of our raycasts
    [SerializeField]
    private float parallelInsetLen;
    [SerializeField]
    private float perpendicularInsetLen;
    [SerializeField]
    private float groundTestLength;
    //this is so you can buffer jumps. i.e. pressing the jump button before you hit the ground and jumping as soon as you hit the ground
    [SerializeField]
    private float jumpInputLeewayPeriod;
    //initial velocity for jumping
    [SerializeField]
    private float jumpStartSpeed;
    [SerializeField]
    private float jumpMaxHoldPeriod;
    [SerializeField]
    private float jumpMinSpeed;

    private Animator animator;

    private Vector2 velocity;

    //our raycasts for movement
    private RaycastMoveDirection moveDown;
    private RaycastMoveDirection moveLeft;
    private RaycastMoveDirection moveRight;
    private RaycastMoveDirection moveUp;

    //our groundcheck
    private RaycastCheckTouching groundDown;

    //for jumping
    private float jumpStartTimer;
    private float jumpHoldTimer;
    private bool jumpInputDown;
    private JumpState jumpState;

    //for animations
    private bool facingRight = true;




    void Start () {

        animator = GetComponent<Animator>();
        /*these weird numbers are setting the starting points for our vectors
        I couldn't (didn't try) get them to be nice numbers because it's based on the 
        current size of our character which is what we're basing everything off of, 
        so I didn't change his size for the sake of design*/
        moveDown = new RaycastMoveDirection(new Vector2(-0.42809565f, -1.37f), new Vector2(0.42809565f, -1.37f), Vector2.down,
            platformMask, Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen);

        moveLeft = new RaycastMoveDirection(new Vector2(-0.42809565f, -1.37f), new Vector2(-0.42809565f, 1.37f), Vector2.left,
            platformMask, Vector2.up * parallelInsetLen, Vector2.right * perpendicularInsetLen);

        moveUp = new RaycastMoveDirection(new Vector2(-0.42809565f, 1.37f), new Vector2(0.42809565f, 1.37f), Vector2.up,
            platformMask, Vector2.right * parallelInsetLen, Vector2.down * perpendicularInsetLen);

        moveRight = new RaycastMoveDirection(new Vector2(0.42809565f, -1.37f), new Vector2(0.42809565f, 1.37f), Vector2.right,
            platformMask, Vector2.up * parallelInsetLen, Vector2.left * perpendicularInsetLen);

        //might need another ground one for all directions for crouching and wall jumping
        groundDown = new RaycastCheckTouching(new Vector2(-0.42809565f, -1.37f), new Vector2(0.42809565f, 1.37f), Vector2.down,
                   platformMask, Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen, groundTestLength);
    }

    //helper function for finding our wanted and velocity directions
    private int GetSign(float input){
        if(Mathf.Approximately(input,0)){
            return 0;
        }
        else if(input > 0){
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private void Update(){

        animator.SetBool("isGrounded", groundDown.DoRaycast(transform.position));
        animator.SetFloat("fallSpeed", velocity.y);

        jumpStartTimer -= Time.deltaTime;
        bool jumpBtn = Input.GetButton("Jump");
        //if you weren't trying to jump last frame, jump!
        if(jumpBtn && jumpInputDown == false){
            jumpStartTimer = jumpInputLeewayPeriod;
        }
        jumpInputDown = jumpBtn;
    }

    private void FixedUpdate()
    {
        switch (jumpState)
        {
            case JumpState.None:
                if (groundDown.DoRaycast(transform.position) && jumpStartTimer > 0)
                {
                    jumpStartTimer = 0;
                    jumpState = JumpState.Holding;
                    jumpHoldTimer = 0;
                    velocity.y = jumpStartSpeed;
                }
                break;
            case JumpState.Holding:
                jumpHoldTimer += Time.deltaTime;
                if(jumpInputDown == false || jumpHoldTimer >= jumpMaxHoldPeriod){
                    jumpState = JumpState.None;
                    //The Lerp function serves to basically always make sure that the player is moving at a minimum speed in the air
                    velocity.y = Mathf.Lerp(jumpMinSpeed, jumpStartSpeed, jumpHoldTimer / jumpMaxHoldPeriod);
                }
                break;
        }


        /*for horizontal movement, the player will ramp up in speed until they hit max
        speed, then they will only move at max speed*/
        float horizInput = Input.GetAxisRaw("Horizontal");
        int wantedDirection = GetSign(horizInput);
        int velocityDirection = GetSign(velocity.x);

        if (wantedDirection != 0)
        {
            //if you want to turn around, you turn basically instantly, hence, SnapSpeed
            if (wantedDirection != velocityDirection)
            {
                velocity.x = horizSnapSpeed * wantedDirection;
            }
            else{
                velocity.x = Mathf.MoveTowards(velocity.x, horizMaxSpeed * wantedDirection, horizAccel * Time.deltaTime);
            }
        }
        else{
            velocity.x = Mathf.MoveTowards(velocity.x, 0, horizDeccel * Time.deltaTime);
        }

        if (jumpState == JumpState.None)
        {   
            //gravity is always pulling down on the player
            velocity.y -= gravity * Time.deltaTime;
        }
        Vector2 displacement = Vector2.zero;
        Vector2 wantedDispl = velocity * Time.deltaTime;

        //for moving our character according to its velocity value
        if(velocity.x > 0){
            displacement.x = moveRight.DoRaycast(transform.position, wantedDispl.x);
        }
        else if(velocity.x < 0){
            displacement.x = -moveLeft.DoRaycast(transform.position, -wantedDispl.x);
        }

        if (velocity.y > 0)
        {
            displacement.y = moveUp.DoRaycast(transform.position, wantedDispl.y);
        }
        else if (velocity.y < 0)
        {
            displacement.y = -moveDown.DoRaycast(transform.position, -wantedDispl.y);
        }

        /*this is incase our player is clipping. If our wanted displacement is not about the same 
        as our actual displacement, then we just flat out stop the player*/
        if(Mathf.Approximately(displacement.x, wantedDispl.x) == false){
            velocity.x = 0;
        }
        if (Mathf.Approximately(displacement.y, wantedDispl.y) == false)
        {
            velocity.y = 0;
        }


        animator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (facingRight == false && velocity.x > 0)
        {
            Flip();
        }
        else if (facingRight == true && velocity.x < 0)
        {
            Flip();
        }


        //this translate function is built into Unity, and it does just that-- moves our character around
        transform.Translate(displacement);
		
	}


    //this changes the x value scale of the player to negative
    //so you can look to the left
    void Flip()
    {

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }

}
