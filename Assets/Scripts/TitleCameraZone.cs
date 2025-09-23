using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cinemachine;
using UnityEngine;

public class TitleCameraZone : CameraZone
{
    public CinemachineVirtualCamera outerCam; //camera to transition to after leaving title zone

    bool done = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //set this cam active at start
        cm.OverrideActiveCam(cam);
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        //do nothing
        return;
    }

    public void Update()
    {
        /*
        //this is silly idgaf
        if (!done && cm.ActiveCam != cam)
        {
            cm.SetActiveCam(cam);
        }
        */
        
    }

    public override void OnTriggerExit2D(Collider2D col)
    {
        if (!done)
        {
            if (col.TryGetComponent<PlayerController>(out PlayerController pc))
            {
                cm.isOverride = false;
                cm.SetActiveCam(outerCam);
                done = true;
            }
        }


    }



    
}
