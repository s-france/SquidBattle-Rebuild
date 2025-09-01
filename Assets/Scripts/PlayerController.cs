using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//🐙
public class PlayerController : MonoBehaviour
{
    PlayerManager pm;

    PhysicsObj phys;
    PlayerInput pi;
    PlayerData data;

    //player state tracking bools
    bool charging = false;
    bool specialCharging = false;
    bool isKnockback = false;

    //


    // Start is called before the first frame update
    void Start()
    {
        pm = FindFirstObjectByType<PlayerManager>();

        phys = GetComponent<PhysicsObj>();
        pi = GetComponent<PlayerInput>();
        data = GetComponent<PlayerData>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

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
            charging = true;

            Debug.Log("Player" + pi.playerIndex + " pressed A");
        }
        else if (ctx.canceled)
        {
            charging = false;

        }

    }

    public void OnBBack(InputAction.CallbackContext ctx)
    {

    }

    public void OnLeftStickAim(InputAction.CallbackContext ctx)
    {

    }

    public void OnLSelectLeft(InputAction.CallbackContext ctx)
    {

    }

    public void OnRSelectRight(InputAction.CallbackContext ctx)
    {

    }


}
