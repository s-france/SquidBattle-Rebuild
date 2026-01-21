using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUpContainer : PlayerContainer
{
    
    // Start is called before the first frame update
    void Start()
    {
         capacity = 6;
    }


    public void OnPlayerEnter(PlayerController pc)
    {
        float pos = -2.5f;

        //set players' positions in a line
        foreach(PlayerController p in Contents)
        {
            p.transform.position.Set(transform.position.x + pos, transform.position.y, transform.position.z);

            pos += 1;
        }

        //run ReadyUp function


    }

    public void OnPlayerExit(PlayerController pc)
    {
        //eject player in a direction

    }
}
