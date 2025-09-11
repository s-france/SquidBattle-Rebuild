using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New PhysicsStats", menuName = "PhysicsStats", order = 1)]
public class PhysicsStats : ScriptableObject
{
    public AnimationCurve MoveCurve;

    //public float maxMoveForce; not in use...

    public AnimationCurve moveSpeedCurve;

    public AnimationCurve movePowerCurve;

    public AnimationCurve moveTimeCurve;

    public float maxMoveSpeed;

    public float maxMoveTime;

    public float maxMovePower;

    public float pushOutPower;



}
