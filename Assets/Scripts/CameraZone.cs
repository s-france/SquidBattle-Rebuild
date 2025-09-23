using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//camera zone trigger for entering/exiting camera zone and enabling/disabling camera for this zone
//GENERAL RULE: camera zone triggers should NOT overlap each other
public class CameraZone : MonoBehaviour
{
    [HideInInspector] public CameraManager cm;
    [HideInInspector] public CinemachineVirtualCamera cam;
    Collider2D zoneTrigger;
    List<PlayerController> contents; //players in this zone

    // Start is called before the first frame update
    public virtual void Start()
    {
        cm = FindFirstObjectByType<CameraManager>();

        cam = GetComponent<CinemachineVirtualCamera>();
        zoneTrigger = GetComponent<Collider2D>();
        contents = new List<PlayerController>();
    }


    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        //if player enters
        if (col.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            //add player to contents
            contents.Add(pc);

            //check if contains all players
            foreach (PlayerInput pi in cm.pm.PlayerList)
            {
                if (!contents.Contains(pi.GetComponent<PlayerController>()))
                {
                    return;
                }
            }

            //set this cam active if all players in zone
            cm.SetActiveCam(cam);


        }


    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            //remove player from contents
            contents.Remove(pc);

            //set active cam to TargetGroup
            cm.SetActiveCam(cm.DefaultCam);



        }

    }

}
