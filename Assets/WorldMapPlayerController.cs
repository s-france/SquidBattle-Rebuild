using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapPlayerController : MonoBehaviour


{
    // Start is called before the first frame update
    [HideInInspector] public Transform Token = null;
    [HideInInspector] private Vector2 MoveVector;

    [HideInInspector] public Transform[] Maps = null;

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
            Debug.Log("A pressed");

            foreach (Transform map in Maps)
            {
                if (Token.GetComponent<Collider2D>().IsTouching(map.GetComponent<Collider2D>()))
                {
                    Debug.Log("True!");
                } else
                {
                    Debug.Log("False!");
                }
            }
        }
    }

    public void OnDecline(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            
        }

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveVector = ctx.ReadValue<Vector2>();
    }
}
