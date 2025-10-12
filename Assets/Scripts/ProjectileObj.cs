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

        return base.Knockback(otherObj);





    }

    

    
}
