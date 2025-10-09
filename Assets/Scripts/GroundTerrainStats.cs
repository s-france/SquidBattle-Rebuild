using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GroundTerrainStats", menuName = "GroundTerrain", order = 1)]
public class GroundTerrainStats : ScriptableObject
{
    public int priority; //priority order for when this terrain overlaps other terrain
    public bool isCheckPoint; //wether or not it is a safe place to respawn to (hub world)

    public float damage; //damage over time dealt to other Objs 
    public float traction; //"slipperiness" of surface -increases moveTime/glideTime -decreases DI influence

    public float timeMod;
    public float speedMod;
    public float powerMod;

    
}
