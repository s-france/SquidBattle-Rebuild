using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Team //: MonoBehaviour
{
    public int idx;
    public int colorIdx;
    public Color color;
    public List<PlayerData> Players;

    public Team(int idx)
    {
        this.idx = idx;
        this.colorIdx = -1;

        this.Players = new List<PlayerData>();
    }

    /*
    public Team(int idx)
    {
    
    }
    */


}
