using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static Match GameMatch;


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

            if (GameMatch == null)
            {
                GameMatch = new Match();
            }


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


    public Match()
    {
        this.gameMode = 0;
        this.scoreMode = 0;
        this.pointsToWin = 10;
        this.ItemsEnabled = new List<int>(); //FINISH THIS: DEFAULT CONSTRUCTOR SHOULD INCLUDE ALL ITEMS!!!

        this.round = 0;

        this.PlayerList = new List<PlayerController>();
        this.Scores = new Dictionary<PlayerController, int>();

        //init player list and scores to 0
        foreach (PlayerInput pi in PlayerManager.Instance.PlayerList)
        {
            this.PlayerList.Add(pi.GetComponent<PlayerController>());
            this.Scores.Add(pi.GetComponent<PlayerController>(), 0);
        }

        
    }

    public Match(int gamemode, int pointstowin, List<int> itemsenabled, int scoremode = 0)
    {
        this.gameMode = gamemode;
        this.scoreMode = scoremode;
        this.pointsToWin = pointstowin;
        this.ItemsEnabled = itemsenabled;

        this.round = 0;

        this.PlayerList = new List<PlayerController>();
        this.Scores = new Dictionary<PlayerController, int>();

        //init player list and scores to 0
        foreach (PlayerInput pi in PlayerManager.Instance.PlayerList)
        {
            this.PlayerList.Add(pi.GetComponent<PlayerController>());
            this.Scores.Add(pi.GetComponent<PlayerController>(), 0);
        }




    }


    public void SetGameMode(int gmode, int smode = 0)
    {
        gameMode = gmode;
        scoreMode = smode;
    }

    public void SetScoreMode(int smode)
    {
        scoreMode = smode;
    }


    public void SetPointsToWin(int points)
    {
        pointsToWin = points;
    }

    public void EnableItem(int item)
    {
        if (!ItemsEnabled.Contains(item))
        {
            ItemsEnabled.Add(item);
        }
    }

    public void DisableItem(int item)
    {
        ItemsEnabled.Remove(item);
    }


}

