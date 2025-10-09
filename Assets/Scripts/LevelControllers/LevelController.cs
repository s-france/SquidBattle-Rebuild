using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    public string levelName;
    public GroundTerrain OOBTerrain; //default "out of bounds" terrain to use when objs are not touching any terrain

    // Start is called before the first frame update
    public virtual void Start()
    {
        Debug.Log("LevelController Start!");

        Instance = this;

        //init important variables
        //PlayerManager.Instance.lc = this;

        OOBTerrain = GetComponent<GroundTerrain>();

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

    public virtual void OnPlayerJoin(PlayerInput pi)
    {

    }

    public virtual void OnPlayerLeave(PlayerInput pi)
    {

    }

    public virtual void SpawnPlayer(int idx)
    {

    }

    



}
