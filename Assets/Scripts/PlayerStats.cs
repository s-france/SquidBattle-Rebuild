using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "PlayerStats", order = 1)]
public class PlayerStats : PhysicsStats
{
    //input windows
    public int wallTechFrameWindow;
    public int parryFrameWindow;

    //charging
    public float maxChargeTime;
    public float minCharge;
    public float maxChargeHoldTime;
    //public float chargeStrength; //not currently in use

    //movement
    public float knockbackMultiplier;
    public float DIStrength;
    public float forwardDIStrength;
    public float lateralDIStrength;
    public AnimationCurve glideDeccelDICurve;
    public float OOBMoveMod;

    //items    
    public int inventorySize;
    public int rewindSize;
    


}
