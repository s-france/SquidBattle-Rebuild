using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerState
{
    public float xPos;
    public float yPos;
    public Vector2 position;
    public float movePower;
    public Vector2 velocity;

    public PlayerState(float xPos, float yPos, float movePower, Vector2 velocity)
    {
        this.xPos = xPos;
        this.yPos = yPos;

        this.position = new Vector2(xPos, yPos);

        this.movePower = movePower;
        this.velocity = velocity;

    }

    public override string ToString()
    {
        return "position: (" + xPos + ", " + yPos + "), power: " + movePower + ", velocity: " + velocity;
    } 



    
}
