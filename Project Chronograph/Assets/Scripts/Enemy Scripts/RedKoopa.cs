using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKoopa : MonoBehaviour {

    public float speed;
    public float distance;

    private bool movingRight = true;
    public Transform groundDetection;
    public Transform wallCheck;
    public float wallRadius;
    public bool hittingWall;
    public LayerMask DefineWall;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, DefineWall);

        transform.Translate(Vector2.right * speed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        if (groundInfo.collider == false || hittingWall == true)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }
}
