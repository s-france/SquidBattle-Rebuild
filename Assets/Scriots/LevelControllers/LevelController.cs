using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    public int levelType;
    [HideInInspector] public PlayerManager pm;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Debug.Log("LevelController Start!");

        //init important variables
        pm = FindFirstObjectByType<PlayerManager>();
        pm.lc = this;

        //default behavior:
        /* //not really needed rn -could be useful at some point to put default behaviors here
        
        //set all players input actionmaps to Gameplay
        foreach (PlayerInput p in pm.PlayerList)
        {
            p.SwitchCurrentActionMap("Gameplay");
        }

        */

    }

    /*
    // Update is called once per frame
    void Update()
    {

    }
    */



    //called at end of level (new scene load)
    public virtual void EndLevel()
    {

    }

    //resets level elements
    public virtual void ResetLevel()
    {

    }

    public virtual void OnPLayerJoin(PlayerInput pi)
    {

    }

    public virtual void OnPlayerLeave(PlayerInput pi)
    {
        
    }

    



}
