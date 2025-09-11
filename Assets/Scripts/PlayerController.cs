using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEditor.Callbacks;


//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

//🐙
public class PlayerController : MonoBehaviour
{
    PlayerManager pm;

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
    [HideInInspector] public bool charging = false;
    [HideInInspector] public float chargeTime = 0;
    [HideInInspector] public bool specialCharging = false;
    [HideInInspector] public float specialChargeTime = 0;
    [HideInInspector] public bool isKnockback = false;


    //


    // Start is called before the first frame update
    void Start()
    {
        pm = FindFirstObjectByType<PlayerManager>();

        phys = GetComponent<PhysicsObj>();
        animate = GetComponent<PlayerAnimation>();
        pi = GetComponent<PlayerInput>();
        data = GetComponent<PlayerData>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        ChargeTick();
        DITick();
    }

    public void OnDeviceLost()
    {
        Debug.Log("P" + pi.playerIndex + " device lost!");
        pm.LeaveTeam(data);

        //unpair device completely cuz we are NOT using lost devices/regained events
        pi.user.UnpairDevices();
    }


    public void OnAConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            chargePressed = true;

            Debug.Log("Player" + pi.playerIndex + " pressed A");
        }
        else if (ctx.canceled)
        {
            chargePressed = false;

            if (!isKnockback && !phys.isHitstop)
            {
                phys.ApplyMove(chargeTime, aim_move);
            }

        }

    }

    public void OnBBack(InputAction.CallbackContext ctx)
    {

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


    //tracks player charging //runs in FixedUpdate()
    void ChargeTick()
    {
        if (chargePressed && !isKnockback && !phys.isMoving)
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

    }


    //processes and applies DI force to physicsObj
    void DITick()
    {

        //exit if not moving or no directional input (no DI)
        if ((!phys.isMoving && !phys.isGliding) || input_move.magnitude == 0) { return; }

        //DI calc vectors
        Vector2 forwardDI = Vector2.zero;
        Vector2 lateralDI = Vector2.zero;
        Vector2 DIMod = Vector2.zero;

        //separate lateral + forward components
        lateralDI = Vector2.Perpendicular(phys.rb.velocity).normalized * Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(phys.rb.velocity, input_move));
        forwardDI = phys.rb.velocity.normalized * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(phys.rb.velocity, input_move));

        //knockback DI modifiers
        if (isKnockback)
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
                phys.glideRate -= (lateralDI.magnitude * .12f);
            }

            //charging glide modifiers
            if (charging)
            {
                //slowmomentum
                phys.glideSpeed = .8f * phys.moveSpeed;
                //extend glide duration to compensate
                phys.glideRate *= .7f;
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
            //apply DI to physObj:
            //TEST THIS!!!!
            phys.rb.velocity *= (phys.rb.velocity.normalized + DIMod).normalized;
        }



    }

    public void ApplyKnockback(PhysicsObj otherObj)
    {


    }

    
    


}
