using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheckTouching {


    private Vector2 raycastDirection;
    private Vector2[] offsetPoints;
    private LayerMask layerMask;
    private float raycastLength;


    /*parallel inset and perpendicular offset also help with positioning the starting points of our raycasts.
    in this case, they are zero because our player doesn't seem to need it. They're pretty much there in case
    we need them in the future*/
    public RaycastCheckTouching(Vector2 start, Vector2 end, Vector2 dir, LayerMask mask, Vector2 parallelInset, Vector2 perpendicularInset, float checkLength)
    {
        this.raycastDirection = dir;

        this.offsetPoints = new Vector2[] {
            start + parallelInset + perpendicularInset,
            end - parallelInset + perpendicularInset};
        this.raycastLength = perpendicularInset.magnitude + checkLength;
        this.layerMask = mask;
    }


    //a copy of the movement raycast function that just checks if you're hitting something and returns what you hit, so you 
    //can move with platforms
    public Collider2D DoRaycast(Vector2 origin)
    {
        foreach (var offset in offsetPoints)
        {
            RaycastHit2D hit = Raycast(origin + offset, raycastDirection, raycastLength, layerMask);

            if (hit.collider != null)
            {
                return hit.collider;
            }

        }
        return null;
    }

    private RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len, LayerMask mask)
    {
        Debug.DrawLine(start, start + dir * len, Color.red);
        return Physics2D.Raycast(start, dir, len, mask);
    }

}
