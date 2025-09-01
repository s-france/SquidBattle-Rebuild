using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D), typeof(Collider2D))]
public class PhysicsObj : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D solidCol;
    [HideInInspector] public CircleCollider2D triggerCol;

    public PhysicsStats stats; //base stats reference for this physics obj

    public UnityEvent<GameObject, GameObject> SolidColission;


    List<Collider2D> Collisions; //list of all colliders currently touching triggerCol
    List<Collider2D> IntangibleColliders; //list of colliders to ignore in collision processing

    //state tracking vars
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isGliding;
    [HideInInspector] public bool isHitstop;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool isIntangible;
    [HideInInspector] public List<Vector2> prevPos; //list of previous positions (used in collision pos correction)
    //

    //current movement state numbers
    float moveSpeed = 0;

    float moveTime = 0;

    float movePower = 0;

    float moveTimer = 0;
    //


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        List<Collider2D> cols = new List<Collider2D>(GetComponents<Collider2D>());

        solidCol = cols.Find(col => !col.isTrigger);
        triggerCol = (CircleCollider2D)cols.Find(col => col is CircleCollider2D && col.isTrigger);

        //init collisions lists
        Collisions = new List<Collider2D>();
        IntangibleColliders = new List<Collider2D>();

        //init prevPos list
        prevPos = new List<Vector2>(3);


    }




    void FixedUpdate()
    {

        MovementTick();

    }


    //runs in FixedUpdate()
    //processes movement
    void MovementTick()
    {
        if (moveTimer >= moveTime)
        {
            moveTime = moveSpeed = movePower = 0;
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = moveSpeed * stats.MoveCurve.Evaluate(moveTimer / moveTime) * rb.velocity.normalized;

            moveTimer += Time.fixedDeltaTime;
        }


    }

    //updates/tracks physics state
    void TrackStateTick()
    {
        //storing previous position for use in trigger collision corrections
        //prevent overfilling
        while (prevPos.Count >= 2)
        {
            prevPos.RemoveAt(0);
        }
        //save previous position
        prevPos.Add(transform.position);
        

    }

    //applys new move - sets movement variables
    void ApplyMove(float moveForce, Vector2 direction) // Assume moveForce is always between 0-1
    {
        moveTimer = 0;
        moveTime = stats.maxMoveTime * stats.moveTimeCurve.Evaluate(moveForce);
        movePower = stats.maxMovePower * stats.movePowerCurve.Evaluate(moveForce);
        moveSpeed = stats.maxMoveSpeed * stats.moveSpeedCurve.Evaluate(moveForce);
        rb.velocity = direction.normalized;
    }


    public void OnTriggerEnter2D(Collider2D col)
    {
        //exit if intangible
        if (IntangibleColliders.Contains(col))
        {
            return;
        }

        //update collisions list
        if (!Collisions.Contains(col))
        {
            Collisions.Add(col);

            //col is another PhysicsObj triggerHB
            if (col.isTrigger && col.TryGetComponent<PhysicsObj>(out PhysicsObj otherObj))
            {
                //physics process collision

                //positional correction
                ///previous position of each colliding physicsObj
                Vector2 prev = prevPos[1];
                Vector2 otherPrev = otherObj.prevPos[1];

                ///collision correction safety check
                if((prev - otherPrev).magnitude > triggerCol.radius + otherObj.triggerCol.radius)
                {
                    //correct collision position
                    var (pos, otherPos) = EstimateCircleTriggerCollision(triggerCol.radius * transform.localScale.x, otherObj.triggerCol.radius * otherObj.transform.localScale.x, transform.position, prev, otherObj.transform.position, otherPrev);

                    Debug.Log("newPos: " + pos);
                    Debug.Log("otherNewPos: " + otherPos);

                    transform.position = pos;
                    otherObj.transform.position = otherPos; //is this needed?
                }

                //apply knockback


            }



        }

    }


    //collision correction recursive loop for when two triggerHBs are supposed to act like solid colliders 
    (Vector2 pos, Vector2 otherPos) EstimateCircleTriggerCollision(float radius1, float radius2, Vector2 pos, Vector2 prev, Vector2 otherPos, Vector2 otherPrev)
    {
        //exit condition
        if(((radius1 + radius2)+.01f >= (prev-otherPrev).magnitude) && ((prev-otherPrev).magnitude >= (radius1 + radius2)-.01f))
        {
            return(prev, otherPrev);
        }


        //recursive divide + conquer loop
        Vector2 dist = pos - prev; //dist points toward pos
        Vector2 otherDist = otherPos - otherPrev;

        //Vector2 newPrev;
        //Vector2 otherNewPrev;

        //plus 1/2 or -1/2 for closer or farther
        if((prev-otherPrev).magnitude > radius1 + radius2)
        {
            prev += (.5f * dist);
            otherPrev += (.5f * otherDist);
        }else if((prev-otherPrev).magnitude < radius1 + radius2)
        {
            prev -= (.5f * dist);
            otherPrev -= (.5f * otherDist);
        }

        //repeat loop
        return EstimateCircleTriggerCollision(radius1, radius2, pos, prev, otherPos, otherPrev);   
    }


    public void OnTriggerExit2D(Collider2D col)
    {
        Collisions.Remove(col);
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        //if other physics obj
        if (col.isTrigger && col.TryGetComponent<PhysicsObj>(out PhysicsObj otherObj))
        {
            if (!isMoving && !isHitstop)
            {
                //push out overlapping triggerHBs
                Vector2 away = (col.transform.position - transform.position).normalized;

                col.transform.position = (Vector2)col.transform.position + (stats.pushOutPower * Time.fixedDeltaTime * away);
            }

        }
        
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        //use messages to conditionally signal other scripts of what's going on
        //gameObject.SendMessage()


    }
}