using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayerController : MonoBehaviour
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
        //pc.Container.OnConfirm(ctx, pc);

    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        pc.Container.OnCancel(ctx, pc);
    }

}
