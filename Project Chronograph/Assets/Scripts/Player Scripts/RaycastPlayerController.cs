using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayerController : MonoBehaviour {

    [SerializeField]
    private LayerMask platformMask;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float parallelInsetLen;
    [SerializeField]
    private float perpendicularInsetLen;


    private Vector2 velocity;
    private RaycastMoveDirection moveDown;

	void Start () {
        /*these weird numbers are setting the starting points for our downwards vectors
        I couldn't (didn't try) get them to be nice numbers because it's based on the 
        current size of our character which is what we're basing everything off of, 
        so I didn't change his size for the sake of design*/
        moveDown = new RaycastMoveDirection(new Vector2(-0.42809565f, -1.37f), new Vector2(0.42809565f, -1.37f), Vector2.down,
            platformMask, Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen);
	}
	
    void FixedUpdate () {
        //basically making our own gravity
        velocity.y -= gravity * Time.deltaTime;

        /*we want the raycast to check down while we're moving down, so we have to flip the velocity 
        we pass through here according to my tutorial lol*/
        float downAmount = moveDown.DoRaycast(transform.position, -velocity.y * Time.deltaTime);

        transform.Translate(new Vector3(0, -downAmount, 0));
		
	}
}
