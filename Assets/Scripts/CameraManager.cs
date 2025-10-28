using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    List<CinemachineVirtualCamera> Cameras; //list of all vCams in scene
    List<CameraZone> CamZones; //list of zone cameras in scene

    public CinemachineVirtualCamera ActiveCam; //currently active vCam
    public CinemachineVirtualCamera DefaultCam; //camera to default to when not in a camerazone (TargetGroupCam in most cases)

    public bool isOverride = false; //allows exception cams to override normal camera switching


    // Start is called before the first frame update
    void Start()
    {


        //fill camera list
        //Cameras = new List<CinemachineVirtualCamera>();
        Cameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList<CinemachineVirtualCamera>();
        CamZones = GetComponentsInChildren<CameraZone>().ToList<CameraZone>();

        //ensure cameras' z pos = -1
        foreach (CinemachineVirtualCamera cam in Cameras)
        {
            cam.transform.position.Set(cam.transform.position.x, cam.transform.position.y, -1);
        }

        SetActiveCam(ActiveCam);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetActiveCam(CinemachineVirtualCamera cam)
    {
        if (isOverride) { return; } //do nothing if overridden

        //deactivate other cams
        foreach (CinemachineVirtualCamera c in Cameras)
        {
            c.Priority = 0;
        }

        //activate this cam
        cam.Priority = 1;
        ActiveCam = cam;
    }

    //sets cam, overriding normal behavior
    public void OverrideActiveCam(CinemachineVirtualCamera cam)
    {
        isOverride = false;

        SetActiveCam(cam);

        isOverride = true;
    }

    //searches for any camera zone containing all players
    public void FindBestCameraZone()
    {
        bool foundCam = false;

        foreach (CameraZone zone in CamZones)
        {
            bool validZone = true;

            //check if zone contains all players
            foreach (PlayerInput pi in PlayerManager.Instance.PlayerList)
            {
                if (!zone.contents.Contains(pi.GetComponent<PlayerController>()))
                {
                    validZone = false;
                    break;
                }
            }

            if (validZone)
            {
                //if found a zone that does contain all players - set as active cam zone
                SetActiveCam(zone.GetComponent<CinemachineVirtualCamera>());
                foundCam = true;
                break;
            }
        }


        //set deafult cam if no valid camzone is found
        if (!foundCam)
        {
            SetActiveCam(DefaultCam);
        }


    }
    


    


}
