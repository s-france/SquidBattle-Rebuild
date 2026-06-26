using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

[CreateAssetMenu(fileName = "New MapCollection", menuName = "MapCollection", order = 1)]

public class MapCollection : ScriptableObject
{
    public string mapname;
    public int subMaps;

}

[CreateAssetMenu(fileName = "New ModeCollection", menuName = "ModeCollection", order = 1)]
public class ModeCollection: ScriptableObject
{
    public string mode;
    public MapCollection[] collections;
}