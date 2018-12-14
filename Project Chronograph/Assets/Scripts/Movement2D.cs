using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This makes sure that whatever the movement script is moving has a collider of some type
[RequireComponent (typeof (Collider2D))] //using just collider2D (I did this to be more general) may cause problems later, I'm not sure, the video used BoxCollider2D but we will see
public class Movement2D : MonoBehaviour {

    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;


    //spacing between each ray
    float horizontalRaySpacing;
    float verticalRaySpacing;

    Collider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;
    
	void Start () {
        collider = GetComponent<Collider2D>();
        CalculateRaySpacing(); //the only time you need to use this function again is if you change the amount of rays mid game
    }

   

    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset();
        if (velocity.x != 0) { 
        HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0){
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    //the ref means that it will change the variable that gets passed through it rather than creating a copy and changing it

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //if we're moving left we want raycasts left. ? means if it's true, set ray origins equal to raycastOrigins.bottomLeft : means if we're moving right then start at bottom right
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

            //changing the ray origin to where you will be after moving on the x axis
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //starts from the bottom left, then moves over one ray space every iteration, and it sends each ray left or right
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //changing x velocity to how much we need to move to get to whatever the raycast hit
                velocity.x = (hit.distance - skinWidth) * directionX;
                //it changes the raylength so the ray will only hit what you're standing on if you're on a ledge or something
                rayLength = hit.distance;

                //setting the collision booleans based on the direction you're moving
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }

        }
    }

    //the ref means that it will change the variable that gets passed through it rather than creating a copy and changing it
    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            //if we're moving up we want raycasts up. ? means if it's true, set ray origins equal to raycastOrigins.bottomLeft : means if we're moving up then start at top left
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

            //changing the ray origin to where you will be after moving on the x axis
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            //starts from the bottom left, then moves over one ray space every iteration, and it sends each ray down
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                //changing y velocity to how much we need to move to get to whatever the raycast hit
                velocity.y = (hit.distance - skinWidth) * directionY;
                //it changes the raylength so the ray will only hit what you're standing on if you're on a ledge or something
                rayLength = hit.distance;

                //setting the collision booleans based on the direction you're moving
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

        }
    }


    void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        //this isn't actually expanding it, it's making it smaller by multiplying the skin width by -2
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = collider.bounds;
        //this isn't actually expanding it, it's making it smaller by multiplying the skin width by -2
        bounds.Expand(skinWidth * -2);

        //making sure you're sending at least 2 horizontal and vertical rays
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //these equation gives equal spacing for however many horizontal raycasts you send out
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    //this helps us easily get the corners of any box collider to start the raycasts.
    //These values won't change, so structs are useful because they can just be copied for any game object.
    struct RaycastOrigins {
    
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public void Reset() {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }

}
