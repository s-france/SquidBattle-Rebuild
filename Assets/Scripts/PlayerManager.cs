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
using Cinemachine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    PlayerInputManager pim;
    [SerializeField] GameObject PlayerPrefab; //player obj prefab

    public ColorSet PlayerColors;

    [HideInInspector] public List<int> TakenColors; //list of taken colors

    [HideInInspector] public List<PlayerInput> PlayerList;
    [HideInInspector] public List<Team> Teams;

    CinemachineTargetGroup CameraTG;



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


            CameraTG = FindFirstObjectByType<CinemachineTargetGroup>();

            //instantiate pim + PLayerPrefab
            pim = GetComponent<PlayerInputManager>();
            pim.playerPrefab = PlayerPrefab;

            //make PlayerList
            PlayerList = new List<PlayerInput>();

            //make Team List
            Teams = new List<Team>();
            //make all 6 teams
            //if you are reading this fuck you
            Teams.Add(new Team(0));
            Teams.Add(new Team(1));
            Teams.Add(new Team(2));
            Teams.Add(new Team(3));
            Teams.Add(new Team(4));
            Teams.Add(new Team(5));


            TakenColors = new List<int>();


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
        if (device is Gamepad && device.added)
        {

            // We execute this code on `playerInput.onControlsChanged`
            if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
            {
                foreach (var item in Gamepad.all)
                {
                    if ((item is UnityEngine.InputSystem.XInput.XInputController) && (Math.Abs(item.lastUpdateTime - device.lastUpdateTime) < 0.1))
                    {
                        Debug.Log($"Switch Pro controller detected and a copy of XInput was active at almost the same time. Disabling XInput device. `{device}`; `{item}`");
                        InputSystem.DisableDevice(item);

                        //ADD THIS: delete item XInput PlayerPrefab + user if user is created


                    }
                }
            }


            // We execute this code on `playerInput.onControlsChanged`
            if (device is UnityEngine.InputSystem.XInput.XInputController)
            {
                foreach (var item in Gamepad.all)
                {
                    if ((item is UnityEngine.InputSystem.Switch.SwitchProControllerHID) && (Math.Abs(item.lastUpdateTime - device.lastUpdateTime) < 0.1))
                    {
                        Debug.Log($"Switch Pro controller detected and a copy of XInput was active at almost the same time. Disabling XInput device. `{device}`; `{item}`");
                        InputSystem.DisableDevice(device);

                        return;
                    }
                }
            }



            foreach (PlayerInput p in PlayerList)
            {
                if (p.hasMissingRequiredDevices)
                {

                    //unpair all devices from player (removes old inactive devices)
                    p.user.UnpairDevices();

                    //assign new device to first deviceless PlayerInput
                    InputUser.PerformPairingWithDevice(device, p.user);

                    //run player joining protocols
                    OnPlayerJoin(p);


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

        PlayerData player = pi.GetComponent<PlayerData>();


        //set PlayerManager as player obj's parent
        pi.transform.parent = transform;

        //add new player to PlayerList
        PlayerList.Add(pi);

        //join camera target group
        CameraTG.AddMember(pi.transform, 1, 1);



        //assign to first empty team
        SetTeam(player, Teams.Find(t => t.Players.Count == 0));

        //assign first available color
        SetColor(player, FindNextAvailableColor(pi.playerIndex, 1));

        //run LevelController player join behavior
        if (LevelController.Instance != null)
        {
            LevelController.Instance.OnPlayerJoin(pi);
        }
        else
        {
            GameObject.FindFirstObjectByType<LevelController>().OnPlayerJoin(pi);
        }

    }

    //called when player leave event triggered
    public void OnPlayerLeave(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " Left!");

        //remove from camera target group
        CameraTG.RemoveMember(pi.transform);

        //remove from PlayerList
        PlayerList.Remove(pi);

        //remove from Team
        LeaveTeam(pi.GetComponent<PlayerData>());

        //trigger lc player leave behavior
        LevelController.Instance.OnPlayerLeave(pi);

    }

    //finds next available player color in direction -1=left 1=right
    //starting from colorID
    public int FindNextAvailableColor(int colorID, int leftRight)
    {
        while (TakenColors.Contains(colorID))
        {
            //move left/right
            colorID += leftRight;

            //wrap around
            if (colorID > PlayerColors.Colors.Count() - 1)
            {
                colorID = 0;
            }
            else if (colorID < 0)
            {
                colorID = PlayerColors.Colors.Count() - 1;
            }
        }

        return colorID;
    }

    public void SetColor(PlayerData player, int colorID)
    {
        if (colorID > PlayerColors.Colors.Count() - 1)
        {
            Debug.Log("ERROR: colorID is out of range!!");
            return;
        }

        //free up old color
        TakenColors.Remove(player.colorIdx);

        //set player's color
        player.colorIdx = colorID;
        player.color = PlayerColors.Colors[colorID];

        //lock down new color
        TakenColors.Add(colorID);


        //set player visuals
        player.SetColor();
    }

    //assigns a player to a team
    public void SetTeam(PlayerData player, Team team)
    {
        //leave current team
        LeaveTeam(player);

        //join new team
        JoinTeam(player, team);
    }

    //adds player to team
    void JoinTeam(PlayerData player, Team team)
    {
        player.Team = team;
        player.teamIdx = team.idx;
        team.Players.Add(player);

        Debug.Log("Player" + player.pi.playerIndex + " joined Team" + team.idx);
    }

    //leaves current teams
    public void LeaveTeam(PlayerData player)
    {
        Debug.Log("Player" + player.pi.playerIndex + " left Team" + player.Team?.idx);
        player.Team?.Players.Remove(player);
        player.teamIdx = -1;
        player.Team = null;
    }




    
    public void SummonPlayers(Transform summonPoint)
    {
        foreach (PlayerInput p in PlayerList)
        {
            StartCoroutine(p.GetComponent<PlayerController>().SummonPlayer(summonPoint));

        }

    }


    

}
