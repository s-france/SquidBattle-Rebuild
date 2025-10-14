using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor.Callbacks;
using UnityEditor.ShaderGraph.Internal;



//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

//🐙
public class PlayerController : MonoBehaviour
{
    WaitForFixedUpdate fuWait; //used in FixedUpdate coroutines

    public PhysicsObj phys;
    PlayerAnimation animate;
    PlayerInput pi;
    PlayerData data;
    public PlayerStats stats;

    //input tracking
    bool chargePressed;
    bool specialChargePressed;
    [HideInInspector] public Vector2 input_move; //current literal left stick input
    [HideInInspector] public Vector2 aim_move; //last non-zero left stick input

    //player state tracking
    [HideInInspector] public Queue<PlayerState> prevStates; //queue of previous states used in rewind function
    [HideInInspector] public PlayerState prevState;  //immediate previous state
    [HideInInspector] public float staticTimer = 0;
    [HideInInspector] public bool charging = false;
    [HideInInspector] public float chargeTime = 0;
    [HideInInspector] public bool specialCharging = false;
    [HideInInspector] public float specialChargeTime = 0;
    [HideInInspector] public bool isRewind = false; //rewind state
    [HideInInspector] public bool isRecall = false; //hub world recovery state
    [HideInInspector] public PlayerState RewindState; //current state of rewind
    [HideInInspector] public float stamina;// = stats.maxStamina;
    [HideInInspector] public PlayerState lastCheckPoint;
    [HideInInspector] public float checkPointTimer = 0; //time since last checkpoint update

    //

    //item stuff
    [HideInInspector] public List<ItemBehavior> ItemInventory; //currently held items
    public int itemInventorySize = 1; //1 item by default
    //


    public static int test;


    // Start is called before the first frame update
    void Start()
    {
        //init fuWait for coroutines
        fuWait = new WaitForFixedUpdate();


        //get gameobj component references
        phys = GetComponent<PhysicsObj>();
        animate = GetComponent<PlayerAnimation>();
        pi = GetComponent<PlayerInput>();
        data = GetComponent<PlayerData>();

        //init prevStates queue to size of max rewind
        prevStates = new Queue<PlayerState>(stats.rewindSize);
        prevState = new PlayerState(transform.position.x, transform.position.y, 0, Vector2.zero, phys.currentTerrain);

        //init checkpoint to spawn pos
        lastCheckPoint = new PlayerState(transform.position.x, transform.position.y, 0, Vector2.zero, phys.currentTerrain);

        //init stamina
        stamina = stats.maxStamina;

        //init items list
        ItemInventory = new List<ItemBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        ChargeTick();
        DITick();

        TrackStateTick();

        RewindTick();
    }

    public void OnDeviceLost()
    {
        Debug.Log("P" + pi.playerIndex + " device lost!");
        PlayerManager.Instance.LeaveTeam(data);

        //unpair device completely cuz we are NOT using lost devices/regained events
        pi.user.UnpairDevices();
    }


    public void OnAConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            chargePressed = true;

            //Debug.Log("Player" + pi.playerIndex + " pressed A");
        }
        else if (ctx.canceled)
        {
            chargePressed = false;

            if (!phys.isKnockback && !phys.isHitStop && !isRewind && phys.moveTimer/phys.moveTime > .4f)
            {
                phys.ApplyMove(false, Mathf.Clamp(chargeTime / stats.maxChargeTime, stats.minCharge, 1), aim_move);

                //update checkpoint status
                if (phys.currentTerrain.stats.isCheckPoint)
                {
                    SetCheckPoint();
                }
            }

        }

    }

    //🅱️ Button = specialCharge
    public void OnBBack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            specialChargePressed = true;
        }
        else if (ctx.canceled)
        {
            specialChargePressed = false;

            //use helditem
            if (!phys.isKnockback && !phys.isHitStop && !isRewind)
            {
                UseItem(ItemInventory[0]);
            }

        }


    }

    public void UseItem(ItemBehavior item)
    {
        ItemInventory.Remove(item);
        item.UseItem(specialChargeTime);
        foreach (ItemBehavior i in ItemInventory)
        {
            int index = ItemInventory.IndexOf(i);
            i.FollowPoint = transform.Find("FollowPoints").GetChild(index);
        }
    }


    public void OnLeftStickAim(InputAction.CallbackContext ctx)
    {
        //update aim vectors
        input_move = ctx.ReadValue<Vector2>();
        if (input_move.magnitude > 0)
        {
            aim_move = input_move;
        }


    }

    public void OnLSelectLeft(InputAction.CallbackContext ctx)
    {

    }

    public void OnRSelectRight(InputAction.CallbackContext ctx)
    {

    }


    void TrackStateTick()
    {
        //
        //Rewind States Tracking
        if (!phys.isHitStop)
        {
            //always store state if moving (Rewind)
            if ((Vector2)transform.position != prevState.position)
            {
                //reset static timer
                staticTimer = 0;

                PlayerState s = new PlayerState(this);
                prevStates.Enqueue(s);

                //update prevState
                prevState = s;
            }
            else
            {
                //store .07s of states when stationary
                if (staticTimer <= .07f)
                {
                    PlayerState s = new PlayerState(this);
                    prevStates.Enqueue(s);
                }
                //track time spent standing still
                staticTimer += Time.fixedDeltaTime;
            }
        }


        //prevent overfilling of prevStates queue
        while (prevStates.Count > stats.rewindSize)
        {
            prevStates.Dequeue();
        }
        //
        //

        //
        //CheckPoint Tracking
        //update checkpoint status if 
        if (!phys.isMoving && phys.currentTerrain.stats.isCheckPoint)
        {
            SetCheckPoint();
        }

        //tick last checkpoint timer
        checkPointTimer += Time.fixedDeltaTime;
        //
        //


        //
        //OOB + Stamina State Tracking
        if (phys.currentTerrain == LevelController.Instance.OOBTerrain) //Out of Bounds
        {
            stamina = Mathf.Clamp(stamina - Time.deltaTime, 0, stats.maxStamina);
        }
        else
        {

            stamina = Mathf.Clamp(stamina + Time.deltaTime, 0, stats.maxStamina);
        }

        if (stamina <= 0 && !isRecall)
        {
            if (checkPointTimer <= 2) //amount of time to rewind vs summons recall
            {
                StartCoroutine(Rewind());
            }
            else
            {
                StartCoroutine(SummonPlayer(lastCheckPoint.position));
            }

        }

        //
        //
    }


    //tracks player charging //runs in FixedUpdate()
    void ChargeTick()
    {

        //normal charge
        if (chargePressed && !phys.isKnockback && !phys.isMoving && !isRewind)
        {
            //if (!isKnockback && !phys.isHitstop /*&& !isRewind*/ )
            //{
            charging = true;
            //}
            //else
            //{
            //    charging = false;
            //}


            //if (charging)
            //{
            chargeTime += Time.fixedDeltaTime;
            //}

        }
        else
        {
            chargeTime = 0;
            charging = false;
        }

        //special charge
        if (specialChargePressed && !phys.isKnockback && !phys.isMoving && !isRewind)
        {
            specialCharging = true;

            specialChargeTime += Time.fixedDeltaTime;
        }
        else
        {
            specialChargeTime = 0;
            specialCharging = false;
        }
    }


    //processes and applies DI force to physicsObj
    void DITick()
    {

        //exit if not moving or no directional input (no DI)
        if ((!phys.isMoving && !phys.isGliding) || input_move.magnitude == 0)
        {
            //safety reset
            phys.glideSpeed = phys.moveSpeed;
            phys.glideRate = 1;

            return;
        }

        //DI calc vectors
        Vector2 forwardDI = Vector2.zero;
        Vector2 lateralDI = Vector2.zero;
        Vector2 DIMod = Vector2.zero;

        //separate lateral + forward components
        lateralDI = Vector2.Perpendicular(phys.rb.velocity).normalized * Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(phys.rb.velocity, input_move));
        forwardDI = phys.rb.velocity.normalized * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(phys.rb.velocity, input_move));


        //knockback DI modifiers
        if (phys.isKnockback)
        {
            DIMod = 1.1f * stats.DIStrength * ((1.1f * stats.lateralDIStrength * lateralDI) + (stats.forwardDIStrength * forwardDI));
        }
        else if (phys.isGliding)
        {
            DIMod = stats.DIStrength * (stats.lateralDIStrength * lateralDI);

            phys.glideRate = 1;

            phys.glideSpeed = phys.moveSpeed;

            float chargeMod = 1;

            //neutral DI + charging
            if (charging && input_move == Vector2.zero)
            {
                phys.glideRate = 1.2f;
            }


            //forwardDI calculations
            if (forwardDI != Vector2.zero)
            {
                //holding backwards
                if (Vector2.Angle(phys.rb.velocity, input_move) > 90)
                {
                    //charge mod
                    if (charging)
                    {
                        chargeMod = 5f;
                    }
                    //extend momentum
                    phys.glideRate = 1 + ((stats.glideDeccelDICurve.Evaluate(forwardDI.magnitude) * .7f) / chargeMod);


                }//holding forwards
                else if (Vector2.Angle(phys.rb.velocity, input_move) < 90)
                {
                    //charge mod
                    if (charging)
                    {
                        chargeMod = 4;
                    }

                    //cut momentum
                    phys.glideRate = 1 - ((forwardDI.magnitude * .3f) / chargeMod);
                }
            }

            //lateralDI calculation
            if (lateralDI != Vector2.zero)
            {
                //extend glide duration
                phys.glideRate -= (lateralDI.magnitude * .1f); //og val: .12
            }

            //charging glide modifiers
            if (charging)
            {
                //slowmomentum
                phys.glideSpeed = .8f * phys.moveSpeed;
                //extend glide duration to compensate
                phys.glideRate *= .8f;
            }
        }
        else if (phys.isMoving)
        {
            //(non-knockback DI ignores forward component)
            DIMod = (/*1.2f * */ stats.lateralDIStrength * lateralDI) * stats.DIStrength;
        }

        //account for physics tick rate
        DIMod *= Time.fixedDeltaTime;


        if (DIMod.magnitude != 0) //band-aid
        {
            //apply DI to physObj: //buss
            phys.rb.velocity = phys.rb.velocity.magnitude * (phys.rb.velocity.normalized + DIMod).normalized;
        }



    }

    public void ApplyKnockback(PhysicsObj otherObj)
    {
        //cancel charge


        //check for parry -> override KB calcs

        //assign killcredit


    }

    //coroutine to summon/warp player to a position (used in hub world transitions)
    public IEnumerator SummonPlayer(Vector2 summonPoint)
    {
        //disable collision while summoning
        phys.solidCol.enabled = false;
        //phys.triggerCol.enabled = false;

        float accelRate;

        //enter recall state
        isRecall = true;


        float timer = 0; //summon timeout timer

        //try to get to position / timeout after 2 sec
        while ((Vector2)transform.position != summonPoint && timer < 2f)
        {
            //acceleration based on distance
            accelRate = stats.summonAccel * ((Vector2)transform.position - summonPoint).magnitude;

            //move toward summon point
            phys.rb.MovePosition(Vector2.MoveTowards(transform.position, summonPoint, stats.summonSpeed * accelRate * Time.fixedDeltaTime));

            timer += Time.fixedDeltaTime;


            yield return fuWait;
        }

        //exit recall state
        isRecall = false;

        //reenable collision
        phys.solidCol.enabled = true;
        //phys.triggerCol.enabled = true;

    }

    public IEnumerator Rewind(float charge = -1)
    {
        //convert state queue to stack
        Stack<PlayerState> states = new Stack<PlayerState>(prevStates.ToArray());

        //clear any immediate stationary frames for more responsive startup
        while (states.Pop().position == (Vector2)transform.position)
        {
            //waiting
        }

        //lock into recall state
        if (charge == -1)
        {
            isRecall = true;
        }

        Vector2 checkPoint = lastCheckPoint.position;

        //enter rewind state
        isRewind = true;
        bool exitCondition = false;

        Vector2 pos = Vector2.zero;

        //disable solid collisions
        phys.solidCol.enabled = false;

        //apply bonus armor
        phys.passiveArmor = 999;

        int tickCount = 0;

        //amount of ticks to rewind for (only used in item version)
        int rewindTicks = (int)(charge / stats.maxChargeTime * stats.rewindSize);


        while (exitCondition == false && states.Count > 0)
        {

            if (!phys.isHitStop)
            {
                RewindState = states.Pop();

                //skip still frames if OOB recovery rewind
                if (charge == -1)
                {
                    while (RewindState.position == (Vector2)transform.position)
                    {
                        //clear still frames
                        RewindState = states.Pop();
                    }
                }

                pos.x = RewindState.xPos;
                pos.y = RewindState.yPos;

                phys.rb.MovePosition(pos);
                phys.storedVelocity = -RewindState.velocity;
                phys.rb.velocity = -RewindState.velocity;
                phys.movePower = Mathf.Clamp(RewindState.movePower, phys.stats.maxMovePower / 5, 999); //minimum movepower for rewind = maxmovepower/5
                phys.movepriority = 4; //invulnerable priority

                //rotate animate
                if (RewindState.velocity.magnitude != 0)
                {
                    animate.RotatePlayer(RewindState.velocity.normalized);
                }

                //increment tick count
                tickCount++;
            }

            //apply bonus armor
            phys.passiveArmor = 999;

            //check exit condition
            //OOB Recall condition
            if (charge == -1)
            {
                exitCondition = (Vector2)transform.position == checkPoint;
            }
            else //item
            {
                exitCondition = (tickCount >= rewindTicks);
            }

            yield return fuWait;
        }

        //reset to normal armor val
        phys.passiveArmor = phys.stats.maxPassiveArmor;

        //exit rewind state
        isRewind = false;
        //exit recall state
        isRecall = false;
        //reenable collisions
        phys.solidCol.enabled = true;
    }

    void RewindTick()
    {
        if (isRewind && !phys.isHitStop)
        {
            Vector2 pos = Vector2.zero;
            pos.x = RewindState.xPos;
            pos.y = RewindState.yPos;

            phys.rb.MovePosition(pos);
            phys.storedVelocity = -RewindState.velocity;
            phys.rb.velocity = -RewindState.velocity;
            phys.movePower = Mathf.Clamp(RewindState.movePower, phys.stats.maxMovePower / 5, 999); //minimum movepower for rewind = maxmovepower/5
            phys.movepriority = 4; //invulnerable priority

            //rotate animate
            if (RewindState.velocity.magnitude != 0)
            {
                animate.RotatePlayer(RewindState.velocity.normalized);
            }
        }
    }

    //called when exiting a piece of ground terrain
    public void ExitTerrain(GroundTerrain terrain)
    {
        /*

        //if exiting a checkpoint zone
        if (terrain.stats.isCheckPoint && !phys.currentTerrain.stats.isCheckPoint)
        {
            //update checkpoint to current pos
            lastCheckPoint.xPos = transform.position.x;
            lastCheckPoint.yPos = transform.position.y;

            //FINISH THIS: replace current pos with Rewind ~ .7sec

            float timer = 0;
            //create temp stack to find previous state
            Stack<PlayerState> previous = new Stack<PlayerState>(prevStates);
            while (timer < .7f)
            {
                //FINISH THIS HERE!!!!
                //count frames in checkpoint terrain
                if (previous.Pop().Terrain.stats.isCheckPoint)
                {
                    timer += Time.fixedDeltaTime;
                }
                else //reset if not in checkpoint > .7fsec
                {
                    timer = 0;
                }
            }

            PlayerState s = previous.Pop();

        }

        */
    }


    //sets a new checkpoint at current position
    public void SetCheckPoint()
    {
        lastCheckPoint.Set(this);
        checkPointTimer = 0;
    }

}
