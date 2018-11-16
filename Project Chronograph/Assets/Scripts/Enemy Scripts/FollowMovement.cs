using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovement : MonoBehaviour {
    private RaycastPlayerController thePlayer;
    public float moveSpeed;
    public float playerRange;
    public TimeManager timeManager;
    public LayerMask playerLayer;
    public bool playerInRange;
    public bool facingAway;
    public bool followOnLookAway;
    private SpriteRenderer spriteRenderer;

    private float timeScalingFactor = 1.0f;

    // Use this for initialization
    void Start () {
        thePlayer = FindObjectOfType<RaycastPlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        playerInRange = Physics2D.OverlapCircle(transform.position, playerRange, playerLayer);

        if (!followOnLookAway)
        {
            if (playerInRange)
            {
                transform.position = Vector3.MoveTowards(transform.position, thePlayer.transform.position, moveSpeed * timeManager.customDeltaTime * timeScalingFactor);
                return;
            }
        }
        if(thePlayer.transform.position.x < transform.position.x && thePlayer.transform.localScale.x < 0)
        {
            facingAway = true;
            spriteRenderer.flipX = false;
        }
        if (thePlayer.transform.position.x > transform.position.x && thePlayer.transform.localScale.x > 0)
        {
            facingAway = true;
            spriteRenderer.flipX = true;
        }
    }
    public void UpdateScale(float scale)
    {
        timeScalingFactor = scale;
    }
}
