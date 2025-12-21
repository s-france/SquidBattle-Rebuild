using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using System.Runtime.Serialization.Formatters;
using NUnit.Framework.Internal;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D), typeof(Collider2D))]
public class PhysicsObj : MonoBehaviour
{
    LevelController lc;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Collider2D solidCol;
    [HideInInspector] public CircleCollider2D triggerCol;

    public PhysicsStats stats; //base stats reference for this physics obj

    //public UnityEvent<GameObject, GameObject> SolidColission;


    List<Collider2D> Collisions; //list of all colliders currently touching triggerCol


    //peer priority tables //i.e. objs to treat as special cases in collisions
    [HideInInspector] public Dictionary<int, float> OverpowerPeerPrioTable; //objs to overpower in KB interactions
    [HideInInspector] public Dictionary<int, float> IntangiblePeerPrioTable; //objs to ignore in collision
    //

    //state tracking vars
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isKnockback = false;
    [HideInInspector] public bool isGliding;
    [HideInInspector] public bool isHitStop;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool isIntangible;
    [HideInInspector] public List<Vector2> prevPos; //list of previous positions (used in collision pos correction)
    [HideInInspector] public Vector2 storedVelocity; //stored true velocity carried through hitstop
    [HideInInspector] public List<GroundTerrain> TerrainContacts;
    [HideInInspector] public GroundTerrain currentTerrain;
    [HideInInspector] public int movepriority;


    //

    //current movement state numbers
    [HideInInspector] public float moveSpeed = 0;

    [HideInInspector] public float moveTime = 0;

    [HideInInspector] public float movePower = 0;

    [HideInInspector] public float moveTimer = 0;


    //glide modifiers
    [HideInInspector] public float glideSpeed = 0;
    [HideInInspector] public float glideRate = 1;
    //

    float glideTime = 0;
    float initialMovePower = 0;
    float glideTimer = 0;
    //


    //hitstop
    [HideInInspector] public float hitStopTime = 0;
    [HideInInspector] public float hitStopTimer = 0;

    //armor system
    [HideInInspector] public float passiveArmor = 0; //actual current passive armor used in KB calc
    [HideInInspector] public float moveArmor = 0; //current moveArmor to be used in KB calcs


    // Start is called before the first frame update
    public void Start()
    {
        lc = FindFirstObjectByType<LevelController>();

        rb = GetComponent<Rigidbody2D>();

        List<Collider2D> cols = new List<Collider2D>(GetComponents<Collider2D>());

        solidCol = cols.Find(col => !col.isTrigger);
        triggerCol = (CircleCollider2D)cols.Find(col => col is CircleCollider2D && col.isTrigger);

        //init collisions lists
        Collisions = new List<Collider2D>();

        //init PeerPriority tables
        OverpowerPeerPrioTable = new Dictionary<int, float>();
        IntangiblePeerPrioTable = new Dictionary<int, float>();

        //init prevPos list
        prevPos = new List<Vector2>(3);

        //init terrain vars
        TerrainContacts = new List<GroundTerrain>();
        currentTerrain = lc.OOBTerrain; //init to OOBTerrain by defualt


    }




    void FixedUpdate()
    {
        TrackStateTick();
        PeerPriorityTick();
        ArmorTick();

        MovementTick();
        HitStopTick();
    }




    //runs in FixedUpdate()
    //processes movement
    public virtual void MovementTick()
    {
        if (!isHitStop)
        {
            if (moveTimer < moveTime) //moving
            {
                isMoving = true;
                isGliding = false;

                //set priority??? (knockbakc)

                //move
                rb.velocity = moveSpeed * stats.MoveCurve.Evaluate(moveTimer / moveTime) * rb.velocity.normalized;
                
                //increment timer
                moveTimer += Time.deltaTime;

            }
            else if (glideTimer < glideTime) //Gliding
            {
                isMoving = false;
                isKnockback = false;
                isGliding = true;

                moveArmor = 0;

                //scale power down based on movecurve progress
                movePower = stats.MoveCurve.Evaluate(1 + (glideTimer / glideTime)) * initialMovePower;

                rb.velocity = stats.MoveCurve.Evaluate(1 + (glideTimer / glideTime)) * glideSpeed * rb.velocity.normalized;

                glideTimer += Time.fixedDeltaTime * glideRate;


            }
            else //done no movement
            {
                isMoving = false;
                isKnockback = false;
                isGliding = false;

                moveArmor = 0;

                glideTime = moveTime = moveSpeed = 0;

                movePower = 0;
                initialMovePower = 0;
                rb.velocity = Vector2.zero;

            }

        }




    }

    //hitstop tracked in FixedUpdate()
    void HitStopTick()
    {
        //in hitstop
        if (hitStopTimer < hitStopTime)
        {
            isHitStop = true;
            rb.velocity = Vector2.zero;
            hitStopTimer += Time.fixedDeltaTime;
        }
        else //not in hitstop
        {
            if (isHitStop) //first frame out of hitstop
            {
                //continue whatever movement is stored
                rb.velocity = storedVelocity;
            }

            //update stored velocity
            storedVelocity = rb.velocity;

            isHitStop = false;
        }

    }

    //updates/tracks state of physicsObj
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

        //update move priority
        movepriority = CalcMovePrio(this);
    }

    //tick armor stats
    //runs in FixedUpdate()
    void ArmorTick()
    {
        if (passiveArmor < stats.maxPassiveArmor)
        {
            passiveArmor = Mathf.Clamp(passiveArmor + Time.fixedDeltaTime, 0, stats.maxPassiveArmor);
        }
        else
        {
            passiveArmor = stats.maxPassiveArmor;
        }

    }

    //applys new move - sets movement variables
    public virtual void ApplyMove(bool isKB, float moveForce, Vector2 direction) //player-inputted moveForce is always between 0-1  //KB force can exceed 1
    {
        isKnockback = isKB;

        //use curves to calc move stats
        moveTime = stats.maxMoveTime * stats.moveTimeCurve.Evaluate(moveForce);
        movePower = initialMovePower = stats.maxMovePower * stats.movePowerCurve.Evaluate(moveForce);
        moveSpeed = glideSpeed = stats.maxMoveSpeed * stats.moveSpeedCurve.Evaluate(moveForce);

        //TRY THIS: only apply mods if !isKB
        //apply terrain mods
        moveTime *= currentTerrain.stats.timeMod;
        movePower *= currentTerrain.stats.powerMod;
        moveSpeed *= currentTerrain.stats.speedMod;

        glideRate = 1;

        glideTime = 2.7f * Mathf.Clamp(moveTime, 0, stats.moveTimeCurve.Evaluate(1.2f));

        moveTimer = 0;
        glideTimer = 0;

        //KB armor = 0
        moveArmor = 0;
        if (!isKB)
        {
            //apply armor if not KB
            moveArmor = stats.maxMoveArmor * stats.armorCurve.Evaluate(moveForce);
        }

        rb.velocity = direction.normalized;
        if (isHitStop) //use storedvelocity if in hitstop
        {
            storedVelocity = direction.normalized;
        }

        //get this man a true
        isMoving = true;

        //update move priority
        movepriority = CalcMovePrio(this);

    }


    //modifies the current movement stats
    //useful for movement impedance/extension/redirection
    //directionMod => modifier applied to current rb direction
    //durationMod => modifier applied to current moveTime + moveTimer vars
    //powerMod => modifier applied to current movePower
    public void ModifyMove(bool isKB, Vector2 directionMod, float durationMod, float speedMod, float powerMod)
    {

        isKnockback = isKB;

        //mod direction
        Vector2 direction;

        if (isHitStop)
        {
            direction = (directionMod + storedVelocity.normalized).normalized;
            storedVelocity = storedVelocity.magnitude * direction;
        }
        else
        {
            direction = (directionMod + rb.velocity.normalized).normalized;
            rb.velocity = rb.velocity.magnitude * direction;
        }
        //

        //mod timers
        moveTime *= durationMod;
        moveTimer *= durationMod;

        //doing too much
        //glideTime *= durationMod;
        //glideTimer *= durationMod;

        moveSpeed *= speedMod;

        //mod power
        movePower *= powerMod;
        initialMovePower *= powerMod;
    }


    //type 0: additive
    //type 1: overwrite
    void ApplyHitStop(int type, float time)
    {

        if (type == 0)
        {
            AddHitStop(time);
        }
        else if (type == 1)
        {
            SetHitStop(time);
        }
    }

    //sets hitstop to time value
    void SetHitStop(float time)
    {
        //init hitstop if not already in hitstop
        if (!isHitStop)
        {
            isHitStop = true;
            storedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
        }


        //overwrite current hitstop to this
        hitStopTime = time;
        hitStopTimer = 0;
    }

    //adds hitstop time on top of current hitstop
    void AddHitStop(float time)
    {
        //init hitstop if not already in hitstop
        if (!isHitStop)
        {
            //use SetHitstop if nothing to add to
            SetHitStop(time);
        }
        else
        {
            //prevent overstacking
            //Mathf.Clamp(hitStopTime, 0, maxHitstop);
            //add extra time to current hitstoptime
            hitStopTime += time;
        }

    }


    public void OnTriggerEnter2D(Collider2D col)
    {

        //update collisions list
        if (!Collisions.Contains(col))
        {
            Collisions.Add(col);

            //col is another PhysicsObj triggerHB
            if (col.isTrigger && col.TryGetComponent<PhysicsObj>(out PhysicsObj otherObj))
            {
                //exit if peer intangible
                //SOMETHING'S WRONG HERE!!!!
                if (IntangiblePeerPrioTable.ContainsKey(otherObj.GetInstanceID()) && IntangiblePeerPrioTable[otherObj.GetInstanceID()] > 0)
                {
                    Debug.Log("intangible collision! Exiting");

                    return;
                }
                Debug.Log("non-intangible collision!");
                Debug.Log("key exists: " + IntangiblePeerPrioTable.ContainsKey(otherObj.GetInstanceID()));



                //physics process collision

                //positional correction
                ///previous position of each colliding physicsObj
                

                Vector2 prev = (prevPos.Count() > 1) ? prevPos[1] : transform.position;
                Vector2 otherPrev = (otherObj.prevPos.Count() > 1) ? otherObj.prevPos[1] : otherObj.transform.position;


                ///collision correction safety check
                if ((prev - otherPrev).magnitude > triggerCol.radius + otherObj.triggerCol.radius) //make sure this is always true!!!
                {
                    //correct collision position
                    var (pos, otherPos) = EstimateCircleTriggerCollision(triggerCol.radius * transform.localScale.x, otherObj.triggerCol.radius * otherObj.transform.localScale.x, transform.position, prev, otherObj.transform.position, otherPrev);

                    transform.position = pos;
                    otherObj.transform.position = otherPos; //is this needed?
                }
                


                //apply knockback
                gameObject.SendMessage("ApplyKnockback", otherObj);

            }



        }

    }


    //collision correction recursive loop for when two triggerHBs are supposed to act like solid colliders 
    (Vector2 pos, Vector2 otherPos) EstimateCircleTriggerCollision(float radius1, float radius2, Vector2 pos, Vector2 prev, Vector2 otherPos, Vector2 otherPrev)
    {
        //exit condition
        if (((radius1 + radius2) + .01f >= (prev - otherPrev).magnitude) && ((prev - otherPrev).magnitude >= (radius1 + radius2) - .01f))
        {
            return (prev, otherPrev);
        }


        //recursive divide + conquer loop
        Vector2 dist = pos - prev; //dist points toward pos
        Vector2 otherDist = otherPos - otherPrev;

        //Vector2 newPrev;
        //Vector2 otherNewPrev;

        //plus 1/2 or -1/2 for closer or farther
        if ((prev - otherPrev).magnitude > radius1 + radius2)
        {
            prev += (.5f * dist);
            otherPrev += (.5f * otherDist);
        }
        else if ((prev - otherPrev).magnitude < radius1 + radius2)
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
            if (!isMoving && !isHitStop)
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


    public void ApplyKnockback(PhysicsObj otherObj)
    {
        StartCoroutine(Knockback(otherObj));
    }

    public virtual IEnumerator Knockback(PhysicsObj otherObj)
    {
        //THIS IS SUS:
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


        //movearmor gets decreased permanently
        float remainingAttack = moveArmor - otherStrength;
        moveArmor = Mathf.Clamp(moveArmor - otherStrength, 0, 10000);

        //passive armor gets decreased by any overflow Strength
        if (moveArmor <= 0 && remainingAttack < 0)
        {
            passiveArmor = Mathf.Clamp(passiveArmor + remainingAttack, 0, 10000);
        }



        //KB calc vars
        float impedanceFactor;
        Vector2 directionMod;

        //first case
        //if this player overpowers otherPlayer
        if (OverpowerPeerPrioTable.ContainsKey(otherObj.GetInstanceID()) && OverpowerPeerPrioTable[otherObj.GetInstanceID()] > 0)
        {
            //overpower "barrel through"
            //impedance based on otherStrength


            //give this player intangible priority from otherPlayer
            //8 ticks of intangibility
            SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);

            //give this player overpower priority over otherPlayer
            SetPeerPriority(OverpowerPeerPrioTable, otherObj, 6 * Time.fixedDeltaTime);

            //alter travel distance
            impedanceFactor = 1 - Mathf.Clamp(1 * otherStrength, .1f, 1);

            //alter direction
            //factors: otherPosDiff, otherRB direction, 
            directionMod = ((2 * otherPosDiff.normalized) + (5 * otherVelocity.normalized)).normalized * Mathf.Clamp((3 * otherStrength), .4f, 1);

            ModifyMove(false, directionMod, impedanceFactor, impedanceFactor, .9f);


            yield break;
        }

        //idk what this means but it's probably important...
        //trying this
        //sort of works, needs tuning (too much KB mitigation)
        otherStrength = .5f * (otherStrength + armoredOtherStrength);
        strength = .5f * (strength + armoredStrength);


        switch (mPrio)
        {
            case -1: //armor deflect

                //give this player intangible priority from otherPlayer
                //EDIT THIS: int constant = invol frame data
                //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);

                //currently in hitstop, use -storedvelocity to cancel out current direction
                directionMod = (Vector2.Reflect(storedVelocity, otherPosDiff) * 200);

                ModifyMove(true, directionMod, 1.1f, .95f, .8f);

                break;
            case 0: //standing still
                if (otherMPrio <= 1) //other standing still / gliding
                {
                    //ADD THIS:
                    //weak gliding KB nudge
                    //use both glidePowers in calc

                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);

                }
                else if (otherMPrio == 2) //other being KB launched
                {
                    //use other player's stats for KB
                    direction = ((otherDirectness * otherImpactDirection) + (otherPosDiff * (1 / otherDirectness))).normalized;


                    //powerful attack KB
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);
                }
                else if (otherMPrio >= 3) //other moving attacking
                {
                    //give this player intangible priority from otherPlayer
                    //EDIT THIS: int constant = invol frame data
                    //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                    SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);
                    
                    //USE OVERPOWER PRIORITY????


                    //use other player's stats for KB
                    direction = ((otherDirectness * otherImpactDirection) + (otherPosDiff * (1 / otherDirectness))).normalized;

                    //TWEAK THIS - calc should be more biased to otherPlayer
                    //powerful attack KB
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);
                }

                break;
            case 1: //gliding
                if (otherMPrio <= 1) //standing still or gliding
                {
                    //ADD THIS:
                    //weak gliding KB nudge
                    //use both glidePowers in calc

                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);

                }
                else if (otherMPrio == 2)
                {
                    //use other player's stats for KB
                    direction = ((otherDirectness * otherImpactDirection) + (otherPosDiff * (1 / otherDirectness))).normalized;

                    //powerful attack KB
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);

                }
                else if (otherMPrio >= 3)
                {
                    //give this player intangible priority from otherPlayer
                    //EDIT THIS: int constant = invol frame data
                    //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                    SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);

                    //use other player's stats for KB
                    direction = ((otherDirectness * otherImpactDirection) + (otherPosDiff * (1 / otherDirectness))).normalized;

                    //powerful attack KB
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);
                }

                break;
            case 2: //knockback launch
                if (otherMPrio <= 1)
                {
                    //give this player intangible priority from otherPlayer
                    //EDIT THIS: int constant = invol frame data
                    //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                    SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);


                    //max possible glidestrength = maxMovePower * .1
                    //alter travel distance
                    impedanceFactor = 1 - Mathf.Clamp(1 * otherStrength, .1f, 1);

                    //alter direction
                    //factors: otherPosDiff, otherRB direction, 
                    directionMod = ((2 * otherPosDiff.normalized) + (5 * otherVelocity.normalized)).normalized * Mathf.Clamp((3 * otherStrength), .4f, 1);

                    ModifyMove(false, directionMod, 1.1f, .95f, 1);


                    //old "8ball" behaviour works for now
                    //^^BRITISH???
                    //ApplyMove(1, direction, knockbackMultiplier * otherStrength);

                    //TRY THIS:
                    //bounce off with slightly increased moveTime
                    //(similar to wallbounce behavior)
                    //ApplyMove(1,)
                    //direction = (velocity.normalized) + Vector2.Reflect(velocity.normalized, otherPosDiff);
                    //ModifyMove(1, direction, 1.1f, .95f, 1);

                    //idk


                }
                else if (otherMPrio == 2)
                {
                    //this should be good (same as 3,3 impact)
                    direction = (((3 * Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2)) * otherImpactDirection) + (otherPosDiff * (1 / Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2))) + (.7f * (1 - (Vector2.Angle(impactDirection, otherImpactDirection) / 180)) * powerRatio * impactDirection)).normalized;

                    //Equal KB exchange
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);

                    //TRY THIS:
                    //impede stronger player's KB more

                }
                else if (otherMPrio >= 3)
                {
                    //TUNE THIS
                    //use other player's direction in calc

                    direction = (((3 * Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2)) * otherImpactDirection) + (otherPosDiff * (1 / Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2))) + (.7f * (1 - (Vector2.Angle(impactDirection, otherImpactDirection) / 180)) * powerRatio * impactDirection)).normalized;

                    //receive full KB
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);
                }

                break;
            case 3: //moving launch
                if (otherMPrio <= 1)
                {
                    //"barrel through"
                    //impedance based on otherStrength

                    //give this player intangible priority from otherPlayer
                    //EDIT THIS: int constant = invol frame data
                    //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                    SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);


                    //give this player overpower priority over otherPlayer
                    //EDIT THIS: int constant = overPower frame data
                    //OverpowerPeerPrioTable[otherPC] = 6 * Time.fixedDeltaTime;
                    SetPeerPriority(OverpowerPeerPrioTable, otherObj, 6 * Time.fixedDeltaTime);

                    //alter travel distance
                    impedanceFactor = 1 - Mathf.Clamp(1 * otherStrength, .1f, 1);

                    //alter direction
                    //factors: otherPosDiff, otherRB direction, 
                    directionMod = ((2 * otherPosDiff.normalized) + (5 * otherVelocity.normalized)).normalized * Mathf.Clamp((3 * otherStrength), .4f, 1);

                    ModifyMove(false, directionMod, impedanceFactor, impedanceFactor, .9f);

                }
                else if (otherMPrio == 2)
                {
                    //ADD THIS
                    //apply reduced knockback based on powerDiff



                    //new direction calc
                    //ADD THIS:
                    //tweak (powerRatio * impactDirection vals)
                    //to allow overpowering players in weak KB launch
                    direction = (((3 * Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2)) * otherImpactDirection) + (otherPosDiff * (1 / Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2))) + (.7f * (1 - (Vector2.Angle(impactDirection, otherImpactDirection) / 180)) * powerRatio * impactDirection)).normalized;


                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);

                }
                else if (otherMPrio >= 3)
                {
                    //ADD THIS: update strength calc
                    //less weight on otherdirectness
                    //add weight from (1/otherDirectness) * this player's moveTimer/moveTime


                    //new direction calc
                    //GOOD:
                    direction = (((3 * Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2)) * otherImpactDirection) + (otherPosDiff * (1 / Mathf.Clamp(otherDirectness, (otherObj.movePower / otherObj.stats.maxMovePower), 2))) + (.7f * (1 - (Vector2.Angle(impactDirection, otherImpactDirection) / 180)) * powerRatio * impactDirection)).normalized;
                    //DONE LFG

                    //Equal KB exchange
                    ApplyMove(true, stats.knockbackMultiplier * otherStrength, direction);
                }

                break;

            case 4: //armored

                if (otherMPrio == -1)
                {

                }
                else
                {
                    //barrel through
                    //"barrel through"
                    //impedance based on otherStrength

                    //give this player intangible priority from otherPlayer
                    //EDIT THIS: int constant = invol frame data
                    //IntangiblePeerPrioTable[otherPC] = 8 * Time.fixedDeltaTime;
                    SetPeerPriority(IntangiblePeerPrioTable, otherObj, 8 * Time.fixedDeltaTime);

                    //give this player overpower priority over otherPlayer
                    //EDIT THIS: int constant = overPower frame data
                    //OverpowerPeerPrioTable[otherPC] = 6 * Time.fixedDeltaTime;
                    SetPeerPriority(OverpowerPeerPrioTable, otherObj, 6 * Time.fixedDeltaTime);

                    //max possible glidestrength = maxMovePower * .1
                    //alter travel distance
                    impedanceFactor = 1 - Mathf.Clamp(.3f * otherStrength, .2f, 1);

                    //alter direction
                    //factors: otherPosDiff, otherRB direction, 
                    directionMod = ((2 * otherPosDiff.normalized) + (4 * otherVelocity.normalized)).normalized * Mathf.Clamp((.5f * otherStrength), .2f, .6f);
                    ModifyMove(false, directionMod, impedanceFactor, impedanceFactor, .9f);
                }


                break;
            default:
                break;
        }



    }


    //calculcates movepriority
    //also useful to get state
    //0 = standing still
    //1 = gliding
    //2 = knockback
    //3 = launch movement
    public int CalcMovePrio(PhysicsObj obj)
    {
        int mPrio = 0;

        if (obj.isMoving)
        {
            mPrio = 3;
            if (obj.isKnockback)
            {
                mPrio = 2;
            }
        }
        else if (obj.isGliding)
        {
            mPrio = 1;
        }

        
        return mPrio;
    }



    public void SetPeerPriority(Dictionary<int, float> prioTable, PhysicsObj otherObj, float time)
    {
        Debug.Log("setting peer prioriorty: " + time);

        if (prioTable.ContainsKey(otherObj.GetInstanceID()))
        {
            prioTable[otherObj.GetInstanceID()] = time;
            Debug.Log("key exists.");
        }
        else
        {
            prioTable.Add(otherObj.GetInstanceID(), time);
            Debug.Log("key does NOT exist!");
        }
    }


    //process Peer Priority updates (FixedUpdate)
    void PeerPriorityTick()
    {
        if (!isHitStop)
        {
            //if done attacking reset overpower priority
            if ((!isMoving || isKnockback) && !isHitStop)
            {
                //clear overpower prio table
                for (int i = 0; i < OverpowerPeerPrioTable.Count; i++)
                {
                    //OverpowerPeerPrioTable.Clear(); //TRY THIS???
                    OverpowerPeerPrioTable[OverpowerPeerPrioTable.Keys.ElementAt(i)] = 0;
                }
            }

            //tick intangibility timers
            for (int i = 0; i < IntangiblePeerPrioTable.Count; i++)
            {
                if (IntangiblePeerPrioTable[IntangiblePeerPrioTable.Keys.ElementAt(i)] > 0)
                {
                    IntangiblePeerPrioTable[IntangiblePeerPrioTable.Keys.ElementAt(i)] -= Time.fixedDeltaTime;
                }

                //clamp at 0
                if (IntangiblePeerPrioTable[IntangiblePeerPrioTable.Keys.ElementAt(i)] < 0)
                {
                    IntangiblePeerPrioTable[IntangiblePeerPrioTable.Keys.ElementAt(i)] = 0;
                }
            }


        }


    }

    //terrain system Enter/Exit (Unity Messages)
    public void EnterTerrain(GroundTerrain terrain)
    {
        //add terrain to contacts list
        TerrainContacts.Add(terrain);

        //if new terrain has highest priority
        if (terrain.stats.priority >= currentTerrain.stats.priority)
        {
            //set new terrain as current
            currentTerrain = terrain;

            //affect current movement //TEST: maaybe not necessary
            ModifyMove(isKnockback, Vector2.zero, terrain.stats.timeMod, terrain.stats.speedMod, terrain.stats.powerMod);
        }
    }

    public void ExitTerrain(GroundTerrain terrain)
    {
        //remove terrain from contacts list
        TerrainContacts.Remove(terrain);

        //if not touching any terrain use OOB Terrain
        if (TerrainContacts.Count() == 0)
        {
            currentTerrain = lc.OOBTerrain;
            return;
        }

        //if leaving current terrain - find+set new current terrain
        if (terrain == currentTerrain)
        {
            //temp priority tracker
            int priority = -1;
            foreach (GroundTerrain t in TerrainContacts)
            {
                if (t.stats.priority >= priority)
                {
                    //set current terrain to that with highest priority
                    currentTerrain = t;
                    priority = t.stats.priority;
                }
            }
        }

    }

    void TerrainTick()
    {

    }

    public void DisableCollision()
    {
        solidCol.enabled = false;
        triggerCol.enabled = false;
    }

    public void EnableCollision()
    {
        solidCol.enabled = true;
        triggerCol.enabled = true;

    }



}