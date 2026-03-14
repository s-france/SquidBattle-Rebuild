using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HubWorldLevelController : LevelController
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        

    }

    /*
    // Update is called once per frame
    void Update()
    {

    }
    */



    //called at end of level (new scene load)
    public override void EndLevel()
    {

    }

    //resets level elements
    public override void ResetLevel()
    {

    }

    public override void OnPlayerJoin(PlayerInput pi)
    {

    }

    public override void OnPlayerLeave(PlayerInput pi)
    {

    }

    public override void SpawnPlayer(int idx)
    {
        
    }


    //transitions to map select screen
    public void OpenMapSelect(Transform map)
    {
        
    }





}
