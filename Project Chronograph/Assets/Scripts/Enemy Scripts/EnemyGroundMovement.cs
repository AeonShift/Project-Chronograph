using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundMovement : MonoBehaviour {

    public float moveSpeed;
    public bool moveRight;
    public Transform wallCheck;
    public float wallCheck2;
    public LayerMask DefineWall;
    private bool hittingWall;
    public TimeManager timeManager;
    private SpriteRenderer spriteRenderer;
    public float timeScalingFactor = 1f;
    public RaycastPlayerController player;
    public TimeZone slowTimeZone;
    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheck2, DefineWall);
        if (hittingWall)
        {
            moveRight = !moveRight;
            
        }

        if (moveRight)
        {
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            if (player.isSlowUsed && slowTimeZone.isAffected)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if(player.isFreezeUsed){
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if(player.isFastUsed && slowTimeZone.isAffected){
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if(slowTimeZone.isAffected){
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed * timeScalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            spriteRenderer.flipX = true;
        }
        else
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (player.isSlowUsed && slowTimeZone.isAffected)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (player.isFreezeUsed)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (player.isFastUsed && slowTimeZone.isAffected)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (slowTimeZone.isAffected)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed * timeScalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed * timeManager.scalingFactor, GetComponent<Rigidbody2D>().velocity.y);
            }
            spriteRenderer.flipX = false;
        }
        
    }

    public void SlowUpdateScale(float scale)
    {
        timeScalingFactor = scale;
    }


}
