using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerManager pm;
    PlayerInput pi;
    PlayerData data;


    // Start is called before the first frame update
    void Start()
    {
        pm = FindFirstObjectByType<PlayerManager>();
        pi = GetComponent<PlayerInput>();
        data = GetComponent<PlayerData>();

    }

    // Update is called once per frame
    void Update()
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
            Debug.Log("Player" + pi.playerIndex + " pressed A");
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
