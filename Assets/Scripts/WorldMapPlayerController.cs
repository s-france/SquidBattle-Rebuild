using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public int Vote = -1;

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Token != null && Vote ==-1)
        {
            Token.position += (Vector3)(MoveVector)*0.25f;
        }
    }

    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("A pressed");
            Collider2D TokenCollider = Token.GetComponent<Collider2D>();
            int index = 0;
            foreach (Transform map in Maps)
            {
                if (TokenCollider.IsTouching(map.GetComponent<Collider2D>()) && Vote == -1)
                {
                    VoteForMap(index);
                    Vote = index;
                }
                else
                {
                    Debug.Log("False!");
                }
                index++;
            }
        }
    }

    public void OnDecline(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Vote != -1)
            {
                UnVoteForMap(Vote);
                Vote = -1;
            }
            else
            {
                // Exit the Scene
            }
        }

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        MoveVector = ctx.ReadValue<Vector2>();
    }

    public void VoteForMap(int Map)
    {
        GameManager.Instance.MapVotes.Add(Map);
    }

    public void UnVoteForMap(int Map)
    {
        GameManager.Instance.MapVotes.Remove(Map);
    }
}

