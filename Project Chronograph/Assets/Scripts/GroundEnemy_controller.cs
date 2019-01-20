using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement2D))]
public class GroundEnemy_controller : MonoBehaviour {

    bool facingRight = false;
    public float moveSpeed = 5;
    public Vector3 velocity;
    float velocityXSmoothing;
    public float accelerationTimeGrounded = .1f;
    public float gravity;

    Movement2D Movement;


	// Use this for initialization
	void Start () {
        Movement = GetComponent<Movement2D>();
        gravity = -gravity;
    }

    // Update is called once per frame
    void Update () {

        if (Movement.collisions.right) {
            moveSpeed = -moveSpeed;
            Flip();
        }
        if(Movement.collisions.left) {
            moveSpeed = -moveSpeed;
            Flip();
        }
        Movement.CalculateGroundEnemyVelocity(ref velocity, moveSpeed, gravity, ref velocityXSmoothing, accelerationTimeGrounded);
        Movement.MoveGroundEnemy(velocity * Time.deltaTime);
        Movement.GravityCollisions(ref velocity);
	}


    void Flip()
    {

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }
}
