using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

    public float moveSpeed;
    public float length;

    public TimeManager timeManager;

    private float initX;
    private float endX;

    private Rigidbody2D rb;
    private Vector2 currentVel;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();

        initX = transform.position.x;
        endX = initX + length;
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = new Vector2 (moveSpeed * timeManager.customDeltaTime, rb.velocity.y);

        if(transform.position.x > endX)
        {
           currentVel = rb.velocity = -newVelocity;
        }
       else if(transform.position.x <= initX)
        {
           currentVel = rb.velocity = newVelocity;
        }
        else
        {
            rb.velocity = currentVel;
        }
    }
}
