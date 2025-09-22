using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stats for a SolidTerrain object
[CreateAssetMenu(fileName = "New SolidTerrainStats", menuName = "SolidTerrain", order = 1)]
public class SolidTerrainStats : ScriptableObject
{
    //public float bounciness; //how far physics objs bounce when colliding with obj
    public float speedMod;
    public float timeMod;
    public float powerMod;

    public float maxHP; //starting hp for this terrain
    public float armor; //armor stat - minimum amount of damage to be counted
    
}
