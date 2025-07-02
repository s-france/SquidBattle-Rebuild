using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    PlayerInputManager pim;
    [SerializeField] GameObject PlayerPrefab; //player obj prefab

    [HideInInspector] public LevelController lc; //current scene's LevelController (updated by LC)


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
            Instance = this;
            DontDestroyOnLoad(Instance);

            //set levelcontroller
            lc = FindFirstObjectByType<LevelController>();
            
            //instantiate pim + PLayerPrefab
            pim = GetComponent<PlayerInputManager>();
            pim.playerPrefab = PlayerPrefab;

            //loop through all currently connected controllers
            ///joining new players for each
            foreach (InputDevice device in InputSystem.devices)
            {
                //check if controller
                if (device is Gamepad)
                {
                    //join new player w device
                    pim.JoinPlayer(pairWithDevice: device);
                }
            }


        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }




    public void OnPlayerJoin()
    {




        lc.OnPLayerJoin();
    }
}
