using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObj : PhysicsObj
{
    //projectile is always moving
    public override void MovementTick()
    {
        if (!isHitStop)
        {
            isMoving = true;
            isKnockback = false;
            isGliding = false;

            //move at constant rate
            rb.velocity = moveSpeed * rb.velocity.normalized;

        }
    }


    public override IEnumerator Knockback(PhysicsObj otherObj)
    {
        //set same movePriority for KB interactions
        movepriority = otherObj.movepriority;
        //^^doesn't work
        //TRY THIS:
        //movepriority = -1;

        Debug.Log("otherobj: " + otherObj.gameObject.name);

        return base.Knockback(otherObj);


    }

    public override void ApplyMove(bool isKB, float moveForce, Vector2 direction)
    {
        Debug.Log("isKB: " + isKB);
        Debug.Log("moveForce: " + moveForce);
        Debug.Log("direction: " + direction);

        if (direction == Vector2.zero)
        {

        }
        
        

        base.ApplyMove(isKB, moveForce, direction);

    }
    

    
}
