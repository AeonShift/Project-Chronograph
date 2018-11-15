//Mostly from this guide: https://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// WHAT NEEDS TO HAPPEN NOW
/// 
/// 1. We need to base the scrolling off of the player's movement one way or another
/// 2. The 'recycled' part of the scrolling background needs to reiterate itself for both +x & -x
/// 3. Probably more than the previous two because this is some  H E A D Y  ass code
/// </summary>

//Parallax scrolling; should be assigned to a layer
public class ScrollingScript : MonoBehaviour
{
    /// <summary>
    /// Public Variables
    /// </summary>
    public Vector2 speed; //Scrolling Speed
    public Vector2 direction; //Direction to scroll (values should be -1, 0, or 1)

    public bool isLinkedToCamera; //Movement should be applied to cam
    public bool isLooping = false; //For infinite backgrounds
    public bool movesWithCam;

    /// <summary>
    /// Private Variables
    /// </summary>
    private List<SpriteRenderer> backgroundPart; //List of children with a renderer

    private Transform cam; //Main cam's transform

    private Vector3 previousCamPos; //Main cam's position last frame

    private void Start()
    {
        //Infinite BG only
        if (isLooping)
        {
            //Get all children of layer with a renderer
            backgroundPart = new List<SpriteRenderer>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                SpriteRenderer r = child.GetComponent<SpriteRenderer>();

                //Add only visible children
                if(r != null)
                {
                    backgroundPart.Add(r);
                }
            }
            //Sort by position, get children from left to right
            //Otherwise we would need more conditions
            backgroundPart = backgroundPart.OrderBy(
                t => t.transform.position.x
                ).ToList();
        }

        //BG follows cam only
        if (movesWithCam)
        {
            cam = Camera.main.transform;
            previousCamPos = cam.position;
        }
    }

    void FixedUpdate()
    {
        if (movesWithCam)
        {
            //Multiply cam position diff by speed to create differing speeds with cam
            float parallax = (previousCamPos.x - cam.position.x) * speed.x;

            transform.position = new Vector3(transform.position.x + parallax, transform.position.y, transform.position.z);
        }
    }

    void Update ()
    {
       
        if(!movesWithCam)
        {
            //Movement
            Vector3 movement = new Vector3(
                speed.x * direction.x,
                speed.y * direction.y,
                0.0f);

            movement *= Time.deltaTime;
            transform.Translate(movement);

            //Move camera
            if (isLinkedToCamera)
            {
                Camera.main.transform.Translate(movement);
            }
        }

        //Making the loop
        if (isLooping)
        {
            //Get first object, list is from left to right (transform.x)
            SpriteRenderer firstChild = backgroundPart.FirstOrDefault();
            SpriteRenderer lastChild = backgroundPart.LastOrDefault();

            if (firstChild != null)
            {
                //Check if child is before the camera
                //Test position first b/c IsVisibleFrom is thicc
                if (firstChild.transform.position.x < Camera.main.transform.position.x)
                {
                    //If child is already left of cam, we test if it's completely out and needs to be re-appended
                    if(!firstChild.IsVisisbleFrom(Camera.main))
                    {
                        Vector3 lastPosition = lastChild.transform.position;
                        Vector3 lastSize = (lastChild.bounds.max - lastChild.bounds.min);

                        //Set position of recycled child AFTER last child
                        //This is only horizontal scrolling (can be modified)
                        firstChild.transform.position = new Vector3(lastPosition.x + lastSize.x, firstChild.transform.position.y, firstChild.transform.position.z);

                        //Recycled child set to last position of backgroundPart list
                        backgroundPart.Remove(firstChild);
                        backgroundPart.Add(firstChild);
                    }
                }
                //Essentially same as before, but for the opposite side
                else if (lastChild.transform.position.x > Camera.main.transform.position.x)
                {
                    if (!lastChild.IsVisisbleFrom(Camera.main))
                    {
                        Vector3 firstPosition = firstChild.transform.position;
                        Vector3 firstSize = (firstChild.bounds.max - firstChild.bounds.min);

                        lastChild.transform.position = new Vector3(firstPosition.x - firstSize.x, lastChild.transform.position.y, lastChild.transform.position.z);

                        backgroundPart.Remove(lastChild);
                        backgroundPart.Insert(0, lastChild);
                    }
                }
            }
        }
	}
}
