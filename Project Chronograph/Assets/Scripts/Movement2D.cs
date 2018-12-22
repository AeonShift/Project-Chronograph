using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This makes sure that whatever the movement script is moving has a collider of some type
[RequireComponent (typeof (Collider2D))] //using just collider2D (I did this to be more general) may cause problems later, I'm not sure, the video used BoxCollider2D but we will see
public class Movement2D : MonoBehaviour {

    public LayerMask collisionMask;
    //for moving things on platforms
    public LayerMask passengerMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    public float maxClimbAngle = 80;
    public float maxDescendAngle = 75;


    //spacing between each ray
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public Collider2D collider;
    public RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    List<PassengerMovement> passengerMovementList;
    //Dictionary is like a Hashset but you can access specific values
    Dictionary<Transform, Movement2D> passengerDictionary = new Dictionary<Transform, Movement2D>();
    

    public void Start () {
        collider = GetComponent<Collider2D>();
        CalculateRaySpacing(); //the only time you need to use this function again is if you change the amount of rays mid game
    }

   
    //if we want the player to move slower in a time zone, we could just create another function called SlowMove where it's basically the same, but the variables are just changed so that the player does everything they normally do slower
    public void MovePlayer(Vector3 velocity, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.x != 0) {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        if (velocity.y <= 0) {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        
        if (velocity.y != 0){
            VerticalCollisions(ref velocity);
        }

        if (standingOnPlatform == true) {
            collisions.below = true;
        }

        transform.Translate(velocity);
    }

    public void JumpPlayer(ref Vector3 velocity, float maxjumpVelocity, float minJumpVelocity, bool wallSliding, int wallDirX, Vector2 wallJumpStrong, Vector2 wallJumpWeak, Vector2 input) {
        if (wallSliding)
        {
            if (wallDirX == input.x)
            {
                velocity.x = -wallDirX * wallJumpStrong.x;
                velocity.y = wallJumpStrong.y;
            }
            else if (input.x == 0)
            {
                velocity.x = -wallDirX * wallJumpWeak.x;
                velocity.y = wallJumpWeak.x;
            }
            else
            {
                velocity.x = -wallDirX * wallJumpStrong.x;
                velocity.y = wallJumpStrong.y;
            }
        }
        
        if (collisions.below)
        {
            velocity.y = maxjumpVelocity;
        }
    }

    public void WallSliding(Vector2 input, ref Vector3 velocity, ref float velocityXSmoothing, float wallSlideSpeedMax, ref bool wallSliding, float wallStickTime, float timeToWallUnstick, int wallDirX) {
        //if you're on a wall and moving down, then you can only move down as fast as the max wall slide speed
        if (collisions.left || collisions.right && !collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        }

        //if you're hitting something above you or below you, velocity will change to zero.
        if (collisions.above || collisions.below)
        {
            velocity.y = 0;
        }
    }

    //Everything having to do with moving platforms and things on platforms
    public void MovePlatform(Vector3 velocity) { 
        transform.Translate(velocity);

    }

    //passing refs through these so that we can change the indexes and waypoints from this script
    public Vector3 CalculatePlatformMovement(float speed, ref int fromWaypointIndex, ref float percentBetweenWaypoints, ref Vector3[] globalWaypoints, bool cyclic, ref float nextMoveTime, float waitTime, float easeAmount) {

        //if the current time is less than the future moveTime, don't move
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        //so that the indexes reset if they go out of bounds
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = PlatformEasing(percentBetweenWaypoints, easeAmount);

        //Lerp, or linear interpolation is a lot easier than it sounds, all it is is finding the line between two points so that you can find a point in between them
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;

    }

    public void MovePassengers(bool beforeMovePlatformInput) {
        foreach (PassengerMovement entity in passengerMovementList) {
            if (!passengerDictionary.ContainsKey(entity.transform)) {
                passengerDictionary.Add(entity.transform, entity.transform.GetComponent<Movement2D>());
            }
            if (entity.moveBeforePlatform == beforeMovePlatformInput) {
                passengerDictionary[entity.transform].MovePlayer(entity.velocity, entity.standingOnPlatform);
            }
        }
    }

    
    public void CalculatePassengerMovement(Vector3 velocity)
    {
        //Hashsets are lists (not programming lists) that are fast at being added to and they are able to be checked through quickly
        //The items in a hashset are unordered and there can be no duplicates the items are all basically given a unique code so it's easy to check if an element exists or not
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovementList = new List<PassengerMovement>();

        
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                //if we're moving up we want raycasts up. ? means if it's true, set ray origins equal to raycastOrigins.bottomLeft : means if we're moving up then start at top left
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

                //changing the ray origin to where you will be after moving on the x axis
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        //Setting the correct values for everything in the passengerMovementList
                        passengerMovementList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }

        }

        //Horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                //if we're moving left we want raycasts left. ? means if it's true, set ray origins equal to raycastOrigins.bottomLeft : means if we're moving right then start at bottom right
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

                //changing the ray origin to where you will be after moving on the x axis
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {

                    if (hit.distance == 0) {
                        //THIS WILL ALLOW THE PLAYER TO WALK THROUGH PLATFORMS THAT PUSH INTO HIM ON A WALL, CHANGE THIS CODE TO KILL THE PLAYER ONCE YOU'VE GOT THAT STUFF
                        continue;
                    }

                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        //Setting the correct values for everything in the passengerMovementList
                        passengerMovementList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        //Passenger ontop of horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {

            //One skinwidth to get to the surface of the platform, then one more to go right above the surface
            float rayLength = skinWidth * 2;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                //We always want to cast from the topleft so that we can catch everything on the platform and changing the ray origin to where you will be after moving on the x axis
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);


                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        //Setting the correct values for everything in the passengerMovementList
                        passengerMovementList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }

            }
        }


    }

    //this function uses an easing function which gives a value according to the percentage of completeness so we can make platforms go slow at the beginning, speed up, then slow at the end
    public float PlatformEasing(float x,float easeAmount) {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    public void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //if velocity.y is larger than climbVelocity, then that means that our player is jumping, so we jump
        if (velocity.y <= climbVelocityY)
        {
            //using trig and treating our velocities as the distance we need to move our Player ( y = d * sin(theta), and x = d * cos(theta))
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }

    }

    public void DescendSlope(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        //if we are descending to the right, we want the downward raycasts to start on the left, and vice versa
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
                if (Mathf.Sign(hit.normal.x) == directionX) {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    //the ref means that it will change the variable that gets passed through it rather than creating a copy and changing it
    public void HorizontalCollisions(ref Vector3 velocity) //if we want different climb angles for enemies or something, make more hori or vert collision functions for them specifically vid is episode 4
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        //so that a little ray will go out from whichever direction you were just moving to detect walls
        if (Mathf.Abs(velocity.x) < skinWidth) {
            rayLength = skinWidth * 2;
        }

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

                //getting the angle of the slope by getting the angle between the normal angle of the slope and the global up.
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //i==0 meaning if the first ray hits the slope
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    //if we're trying to run up a new slope, adjust our character's spacing from the slope
                    if (slopeAngle != collisions.slopeAngleOld) {
                        //so that our character is actually sticking to the slope
                        distanceToSlopeStart = hit.distance - skinWidth;
                        //we subtract so that when we call ClimbSlope, we will be using the velocity we had before we touched the slope
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    //so that we keep sticking to the slope
                    velocity.x += distanceToSlopeStart * directionX;
                }


                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) { 
                //changing x velocity to how much we need to move to get to whatever the raycast hit
                velocity.x = (hit.distance - skinWidth) * directionX;
                //it changes the raylength so the ray will only hit what you're standing on if you're on a ledge or something
                rayLength = hit.distance;

                    //so that you don't jitter up and down if you run into something while climbing a slope
                    if (collisions.climbingSlope) {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                //setting the collision booleans based on the direction you're moving
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
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
                if (hit.collider.tag == "Through") {
                    if (directionY == 1) {
                        continue;
                    }
                }
                //changing y velocity to how much we need to move to get to whatever the raycast hit
                velocity.y = (hit.distance - skinWidth) * directionY;
                //it changes the raylength so the ray will only hit what you're standing on if you're on a ledge or something
                rayLength = hit.distance;

                if (collisions.climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                //setting the collision booleans based on the direction you're moving
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

        }

        //checking if there is a new slope where we will be on the y axis next frame so we don't accidentally move into an object
        if (collisions.climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            //if we're moving left, start the raycast on the bottom left, if moving right, start bottom right adding vector2.up multiplied by our current y velocity so that we will check at our new height
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                //we need to check the next slope angle with the current slope angle, so we make a new one (this one will be the future slope angle)
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }    

            }

        }

    }


    public void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        //this isn't actually expanding it, it's making it smaller by multiplying the skin width by -2
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing() {
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
    public struct RaycastOrigins {
    
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle;
        public float slopeAngleOld;
        public Vector3 velocityOld;
        public int faceDir;

        public void Reset() {
            above = false;
            below = false;
            left = false;
            right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    //This struct will store all the variables needed for moving passengers
    public struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;
        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;

        }


    }

    
}
