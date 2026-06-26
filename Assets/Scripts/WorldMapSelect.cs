using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;


public class WorldMapSelect : MonoBehaviour
{
    public Transform[] Tokens;
    // Start is called before the first frame update
    /*
    void Start()
    {
        
    }
    */

    // Update is called once per frame
    void Update()
    {
        TestFunction();
    }

    public void Activate(PlayerContainer container)
    {
        // switch players' actionmap to WorldMap controls
        foreach (PlayerInput player in PlayerManager.Instance.PlayerList)
        {
            player.SwitchCurrentActionMap("WorldMap");
        }
        // Assign Players to Token
        foreach (Transform Token in Tokens)
        {
            
        }

        //init display
        //set default token pos
        //draw all sprites


    }

    public void Deactivate(PlayerContainer container)
    {

    }

    void TestFunction() {
        Debug.Log(Tokens[0].GetComponent<Transform>().position.x);
    }
}
