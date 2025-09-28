using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GroundTerrainStats", menuName = "GroundTerrain", order = 1)]
public class GroundTerrainStats : ScriptableObject
{
    public int priority; //priority order for when this terrain overlaps other terrain

    public float damage; //damage over time dealt to other Objs 
    public float traction; //"slipperiness" of surface -increases moveTime/glideTime -decreases DI influence

    
}
