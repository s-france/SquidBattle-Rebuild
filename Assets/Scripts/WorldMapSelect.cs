using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;


public class WorldMapSelect : MonoBehaviour
{
    public Transform[] Tokens;
    public Transform[] Maps;
    public GameObject cam;
    public GameObject map;

    [HideInInspector] public PlayerContainer originContainer; //player container that opened map screen
    
    // Start is called before the first frame update
    
    void Start()
    {
        Deactivate();
    }


    // Update is called once per frame
    void Update()
    {
        TestFunction();
    }

    public void Activate(PlayerContainer container = null)
    // public void Activate()
    {
        Debug.Log("activating world map!");

        //activate camera

        cam.SetActive(true);
        map.SetActive(true);

        // switch players' actionmap to WorldMap controls
        int index = 0;
        foreach (PlayerInput player in PlayerManager.Instance.PlayerList)
        {
            player.SwitchCurrentActionMap("WorldMap");
            Tokens[index].position = Vector3.zero;
            player.GetComponent<WorldMapPlayerController>().Token = Tokens[index];
            index++;
        }

        //Debug.Log("i = " + index);

        while (index < 6)
        {
            //Debug.Log("disabling token " + index);
            Tokens[index].gameObject.SetActive(false);
            index++;
        }


        //set default token pos
        //draw all sprites


    }

    public void Deactivate(PlayerContainer container = null)
    //public void Deactivate()
    {
        cam.SetActive(false);
        map.SetActive(false);
    }

    void TestFunction() {
        //Debug.Log(Tokens[0].GetComponent<Transform>().position.x);
    }
}
