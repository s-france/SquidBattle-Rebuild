using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ReadyUpContainer : PlayerContainer
{
    public UnityEvent playersReadyEvent;

    // Start is called before the first frame update
    void Start()
    {
        capacity = 6;
    }


    public void OnPlayerEnter(PlayerController pc)
    {
        Debug.Log("Player" + pc.data.inGameIdx + " entered ReadyContainer!");
        float pos = -2.5f;

        //set players' positions in a line
        foreach(PlayerController p in Contents)
        {
            p.transform.position.Set(transform.position.x + pos, transform.position.y, transform.position.z);

            pos += 1;
        }

        //run ReadyUp function
        

        //if all active players are entered/ready -> run ready event
        if (Contents.Count == PlayerManager.Instance.PlayerList.Count)
        {
            //set event behavior in inspector using GameManager
            playersReadyEvent.Invoke();
        }

    }

    public void OnPlayerExit(PlayerController pc)
    {
        Contents.Remove(pc);
        

        //eject player in a direction
        pc.phys.ApplyMove(true, EjectStat.magnitude, EjectStat);

    }



    
}
