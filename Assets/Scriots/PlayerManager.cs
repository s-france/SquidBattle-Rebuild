using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;
//using UnityEngine.iOS;
using UnityEngine.InputSystem.Users;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    PlayerInputManager pim;
    [SerializeField] GameObject PlayerPrefab; //player obj prefab

    [HideInInspector] public LevelController lc; //current scene's LevelController (updated by LC)

    List<PlayerInput> PlayerList;



    // Start is called before the first frame update
    void Start()
    {



    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("ERROR: trying to make more than one PlayerManager!");
        }
        else
        {
            //init as static instance
            Instance = this;
            DontDestroyOnLoad(Instance);

            //set levelcontroller (future lc's set by lc in scene)
            lc = FindFirstObjectByType<LevelController>();

            //instantiate pim + PLayerPrefab
            pim = GetComponent<PlayerInputManager>();
            pim.playerPrefab = PlayerPrefab;

            //make PlayerList
            PlayerList = new List<PlayerInput>();

            //device-change tracking event
            InputSystem.onDeviceChange +=
            (device, change) =>
            {
                //only using gamepads
                if (device is Gamepad)
                {
                    switch (change)
                    {
                        //new device added
                        case InputDeviceChange.Added:
                            Debug.Log(device.displayName + " added");
                            Debug.Log(device.displayName + " ID " + device.deviceId);
                            Debug.Log(device.description.serial);
                            Debug.Log(device.path);
                            Debug.Log(device.ToString());


                            AssignNewDeviceToPlayer(device);
                            
                            break;

                        case InputDeviceChange.Removed:
                            Debug.Log(device.displayName + " removed");
                            Debug.Log(device.displayName + " ID " + device.deviceId);
                            Debug.Log(device.description.serial);
                            Debug.Log(device.path);
                            Debug.Log(device.ToString());


                            break;

                        case InputDeviceChange.Disconnected:
                            Debug.Log(device.displayName + " disconnected");
                            Debug.Log(device.displayName + " ID " + device.deviceId);
                            Debug.Log(device.description.serial);
                            Debug.Log(device.path);
                            Debug.Log(device.ToString());

                            break;

                        case InputDeviceChange.Reconnected:
                            Debug.Log(device.displayName + " reconnected");
                            Debug.Log(device.displayName + " ID " + device.deviceId);
                            Debug.Log(device.description.serial);
                            Debug.Log(device.path);
                            Debug.Log(device.ToString());

                            break;
                    }




                }

            };


            //loop through all currently connected controllers
            ///joining new players for each
            foreach (Gamepad device in Gamepad.all)
            {
                AssignNewDeviceToPlayer(device);
                
            }


        }

    }

    // Update is called once per frame
    void Update()
    {


    }



    //checks if a newly connected device should create a new player
    //true - call pim.JoinPlayer
    //false - assign device to first player with no device
    void AssignNewDeviceToPlayer(InputDevice device)
    {
        if (device is Gamepad)
        {
            foreach (PlayerInput p in PlayerList)
            {
                if (p.hasMissingRequiredDevices)
                {
                    //assign device to first deviceless PlayerInput
                    InputUser.PerformPairingWithDevice(device, p.user);

                    //exit
                    return;
                }
            }

            //if all players already have paired devices
            //join a new player with new device
            pim.JoinPlayer(pairWithDevice: device);
            return;


        }
        else
        {
            Debug.Log("connected device is NOT a compatible gamepad!");
            return;
        }
    }




    //called when player join event triggered
    public void OnPlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " Joined!");

        //add new player to PlayerList
        PlayerList.Add(pi);

        //run LevelController player join behavior
        lc.OnPLayerJoin();
    }

    //called when player leave event triggered
    public void OnPlayerLeave(PlayerInput pi)
    {

    }

    

}
