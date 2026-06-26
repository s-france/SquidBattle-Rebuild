using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapPlayerController : MonoBehaviour


{
    // Start is called before the first frame update
    [HideInInspector] public Transform Token = null;
    [HideInInspector] private Vector2 MoveVector;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Token != null)
        {
            Token.position += (Vector3)MoveVector;
        }
    }

    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        { 
            
        }
    }

    public void OnDecline(InputAction.CallbackContext ctx)
    {

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveVector = ctx.ReadValue<Vector2>();
    }
}
