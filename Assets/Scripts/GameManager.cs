using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("ERROR: trying to make more than one GameManager!");
        }
        else
        {
            //init as static instance
            Instance = this;
            DontDestroyOnLoad(Instance);

            
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class Match
{
    public int gameMode; //gamemode being played
    public int scoreMode; //score calculation mode (for modes with different scoring types)
    public List<PlayerController> PlayerList; //list of players in match
    public Dictionary<PlayerController, int> Scores; //scorekeeping dictionary
    public int round = 0; //current round number
    public int pointsToWin; //points to be considered the winner
    public List<int> ItemsEnabled; //list of enabled items to spawn


}

