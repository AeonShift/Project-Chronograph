using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {

    public float speed = 8;  // how fast you can move
    public float jumpforce = 18; // how high you can jump
    private float moveInput; // if the left or right keys are pressed


    private Rigidbody2D rb;
    public Animator animator;

    private bool facingRight = true;
    bool crouch = false;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    private int presses = 0;

    public TimeManager timeManager;
    public PlayerAttack playerAttack;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
		
	}

    // Use this for physics
    void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
       


        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        animator.SetFloat("Speed", Mathf.Abs(moveInput * speed));

        if(facingRight == false && moveInput > 0){
            Flip();
        } else if(facingRight == true && moveInput < 0){
            Flip();
        }

    }

    private void Update()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("fallSpeed", rb.velocity.y);



        if (Input.GetButtonDown("Jump"))
        {

            if (isGrounded == true)
            {
                rb.velocity = Vector2.up * jumpforce;
            }
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if(Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("SlowButton") && presses == 0) 
        {
            presses += 1;
            StartCoroutine(Slow());
        }

        if (Input.GetButtonDown("SpeedButton") && presses == 0) {

            presses += 1;
            StartCoroutine(Speed());
        }

        if (Input.GetButtonDown("FreezeButton") && presses == 0) {

            presses += 1;
            StartCoroutine(Freeze());

        }
    }

    IEnumerator Slow() {

        float timePassed = 0;

        while (timePassed < 3) {

            timeManager.Slowdown();
            timePassed += Time.deltaTime;

            yield return null;


        }
        timeManager.UndoTime();
        presses = 0;


    }

    IEnumerator Speed()
    {

        float timePassed = 0;

        while (timePassed < 3)
        {

            timeManager.Speedup();
            timePassed += Time.deltaTime;

            yield return null;


        }
        timeManager.UndoTime();
        presses = 0;


    }

    IEnumerator Freeze()
    {

        float timePassed = 0;

        while (timePassed < 3)
        {

            timeManager.Freeze();
            timePassed += Time.deltaTime;

            yield return null;


        }
        timeManager.UndoTime();
        presses = 0;


    }

    //this changes the x value scale of the player to negative
    //so you can look to the left
    void Flip(){

        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }


    //to have the player move with the platform
    void OnCollisionEnter2D(Collision2D other){

        if(other.transform.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
        if (other.gameObject.tag == "Zone")
        {
            Debug.Log("please dude");
        }
    }

    //to have the player move with the platform
    void OnCollisionExit2D(Collision2D other)
    {

        if (other.transform.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

        


    }

