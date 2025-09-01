using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CSSPlayerController : MonoBehaviour
{
    PlayerManager pm;
    PlayerData data;
    CSSLevelController lc;

    [HideInInspector] public CSSWindowDisplay UIWindow;


    [HideInInspector] public bool isReady = false; //ReadyUp state

    // Start is called before the first frame update
    void Start()
    {
        pm = FindFirstObjectByType<PlayerManager>();
        lc = FindFirstObjectByType<CSSLevelController>();
        data = GetComponent<PlayerData>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnASelect(InputAction.CallbackContext ctx)
    {


    }

    public void OnBCancel(InputAction.CallbackContext ctx)
    {

    }

    public void OnLeftL(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isReady)
        {
            pm.SetColor(data, pm.FindNextAvailableColor(data.colorIdx, -1));

        }

    }

    public void OnRightR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isReady)
        {
            pm.SetColor(data, pm.FindNextAvailableColor(data.colorIdx, 1));

        }

    }

    public void OnLeftTrig(InputAction.CallbackContext ctx)
    {

    }

    public void OnRightTrig(InputAction.CallbackContext ctx)
    {
        
    }

    public void OnHoldBack(InputAction.CallbackContext ctx)
    {

    }

    public void OnJoinLR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && UIWindow == null)
        {
            foreach (CSSWindowDisplay window in lc.PlayerWindows)
            {
                if (window.playerIdx == -1)
                {
                    AssignUIWindow(window);
                    break;
                }
            }
        }


        


    }

    public void OnShoulderJoinRTLT(InputAction.CallbackContext ctx)
    {

    }




    //assigns this player to a CSS window
    public void AssignUIWindow(CSSWindowDisplay window)
    {
        UIWindow = window;

        window.playerIdx = data.pi.playerIndex;
        data.inGameIdx = Array.IndexOf(lc.PlayerWindows, window);

        window.JoinPlayer();
    }

    //unassigns UI window - unjoins player from CSS
    public void UnassignUIWindow()
    {
        if (UIWindow != null)
        {
            UIWindow.LeavePlayer();
            UIWindow.playerIdx = -1;
        }

        
        data.inGameIdx = -1;

        UIWindow = null;
    }
}
