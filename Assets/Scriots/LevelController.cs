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


        pm = FindFirstObjectByType<PlayerManager>();
        pm.lc = this;

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

    public virtual void OnPLayerJoin()
    {
        
    }

    



}
