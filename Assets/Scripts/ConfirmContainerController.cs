using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Input controller class attached to a player for use with Confirm type Containers
public class ConfirmContainerController : MonoBehaviour
{
    PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            //machine behavior script receives confirm input
            pc.Container.SendMessage("OnConfirm");
        } else if(ctx.canceled)
        {
            
        }
        
    }
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            pc.Container.SendMessage("OnExit");
        } else if(ctx.canceled)
        {
            
        }
        
    }
}
