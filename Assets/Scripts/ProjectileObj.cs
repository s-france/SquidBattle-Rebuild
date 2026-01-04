using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObj : PhysicsObj
{
    //projectile is always moving
    float speed;

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
        //return base.Knockback(otherObj);


        //NEW: THIS IS WHAT WE'RE DOING

        //reflect off collision normal for bounce off KB direction

        //maintain same speed+power stats only change direction


        //THIS IS SUS:  /you can probably just delete/remove this now...
        //baseline peer intangibility to prevent double collisions
        SetPeerPriority(IntangiblePeerPrioTable, otherObj, .2f);

        //true velocity at impact
        Vector2 velocity = isHitStop ? storedVelocity : rb.velocity;
        Vector2 otherVelocity = otherObj.isHitStop ? otherObj.storedVelocity : otherObj.rb.velocity;

        //move priorities
        int mPrio = movepriority;
        int otherMPrio = otherObj.movepriority;


        //difference in priority
        int priorityDiff = mPrio - otherMPrio;

        //difference in powers
        float powerDiff = movePower - otherObj.movePower;
        //power ratio
        float powerRatio = movePower / otherObj.movePower;

        //angle players are colliding:
        //1 = moving straight at each other
        //0 = moving in same direction
        float directionDiff = Vector2.Angle(velocity, otherVelocity) / 180;

        //relative positions
        //vector pointing from player's position to otherPlayer's position i.e. relative position (direction only)
        Vector2 posDiff = (otherObj.transform.position - transform.position).normalized;
        //vector pointing from otherPlayer's position to this player's position
        Vector2 otherPosDiff = -posDiff;  //(transform.position - otherObj.transform.position).normalized;


        //how much of a "direct hit" it is
        //1 = direct hit
        //0 = indirect hit
        //animationCurve used to level out "almost direct hits" and keep value > 0
        float directness = 0;
        if (velocity != Vector2.zero)
        {
            directness = stats.directnessKBCurve.Evaluate(1 - (Vector2.Angle(posDiff, velocity) / 180));
        }

        float otherDirectness = 0;
        if (otherVelocity != Vector2.zero)
        {
            otherDirectness = otherObj.stats.directnessKBCurve.Evaluate(1 - (Vector2.Angle(otherPosDiff, otherVelocity) / 180));
        }

        float directnessRatio = otherDirectness / directness;


        //overall strength: takes directness and power into account
        float strength = directness * movePower;
        float otherStrength = otherDirectness * otherObj.movePower;

        //armor calcs
        //strength after subtracting armor mitigation
        float armoredStrength = strength - (otherObj.passiveArmor + otherObj.moveArmor) + (passiveArmor + moveArmor);
        float armoredOtherStrength = otherStrength - (passiveArmor + moveArmor) + (otherObj.passiveArmor + otherObj.moveArmor);
        //new armor vals after subtracting impact strength
        float armor = (passiveArmor + moveArmor) - otherStrength;
        float otherArmor = (otherObj.passiveArmor + otherObj.moveArmor) - strength;

        float strengthDiff = otherStrength - strength;
        float strengthRatio = otherStrength / strength;



        //calc hitstop
        float hitstop = (strength > otherStrength) ? (strength / stats.maxMovePower) : (otherStrength / otherObj.stats.maxMovePower);


        Vector2 impactDirection = velocity.normalized;
        Vector2 otherImpactDirection = otherVelocity.normalized;

        //calculate knockback direction
        Vector2 direction;
        //direction when one player overpowers other               
        direction = ((otherDirectness * otherImpactDirection) + (otherPosDiff * (1 / otherDirectness))).normalized;



        //armor priority recalculations
        //other player is armored
        if (otherArmor > 0 && otherArmor > armor)
        {
            if (otherMPrio <= 1)
            {
                //deflected
                mPrio = -1;
            }
            else if (mPrio > 0)
            {
                //overpowered
                mPrio = 1;
            }

            //armor overpower
        }
        else if (armor > 0)
        {
            if (otherMPrio <= 1)
            {

            }
            else
            {
                //overpower
                mPrio = 4;
            }
        }



        //apply impact hitstop
        ApplyHitStop(1, stats.maxHitStop * stats.hitStopCurve.Evaluate(hitstop));

        /////
        //wait one tick so both sides' KB calcs can finish
        yield return new WaitForFixedUpdate();
        /////


        




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
