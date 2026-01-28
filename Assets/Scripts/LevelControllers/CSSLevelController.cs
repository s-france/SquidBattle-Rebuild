using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//THIS IS OLD WE'RE NOT USING THIS ANYMORE CSS HAS BEEN REPLACED BY HUB WORLD
public class CSSLevelController : MenuLevelController
{
    public CSSWindowDisplay[] PlayerWindows; //PlayerWindow UI controls

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        //PlayerWindows = FindObjectsByType<CSSWindowDisplay>(FindObjectsSortMode.None);


        foreach (PlayerInput p in PlayerManager.Instance.PlayerList)
        {
            //set all players input actionmaps to CharacterSelect
            p.SwitchCurrentActionMap("CharacterSelect");
            //p.GetComponent<CSSPlayerController>().UIWindow = PlayerWindows[p.playerIndex];

        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPlayerLeave(PlayerInput pi)
    {
        base.OnPlayerLeave(pi);

        //free up player's UI window
        pi.GetComponent<CSSPlayerController>().UnassignUIWindow();


    }

    public override void OnPlayerJoin(PlayerInput pi)
    {
        pi.SwitchCurrentActionMap("CharacterSelect");
    }
}
