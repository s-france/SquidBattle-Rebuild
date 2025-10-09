using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    List<CinemachineVirtualCamera> Cameras; //list of vCams in scene
    public CinemachineVirtualCamera ActiveCam; //currently active vCam
    public CinemachineVirtualCamera DefaultCam; //camera to default to when not in a camerazone (TargetGroupCam in most cases)

    public bool isOverride = false; //allows exception cams to override normal camera switching


    // Start is called before the first frame update
    void Start()
    {


        //fill camera list
        Cameras = new List<CinemachineVirtualCamera>();
        Cameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList<CinemachineVirtualCamera>();

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
    


    


}
