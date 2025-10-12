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

    public GroundTerrain Terrain;

    public PlayerState(float xPos, float yPos, float movePower, Vector2 velocity, GroundTerrain terrain)
    {
        this.xPos = xPos;
        this.yPos = yPos;

        this.position = new Vector2(xPos, yPos);

        this.movePower = movePower;
        this.velocity = velocity;

        this.Terrain = terrain;
    }

    public PlayerState(PlayerController pc)
    {
        this.xPos = pc.transform.position.x;
        this.yPos = pc.transform.position.y;

        this.position = pc.transform.position;

        this.movePower = pc.phys.movePower;
        //ADD STOREDVELOCITY CHECK HERE
        this.velocity = pc.phys.rb.velocity;

        this.Terrain = pc.phys.currentTerrain;
    }

    public void Set(PlayerController pc)
    {
        this.xPos = pc.transform.position.x;
        this.yPos = pc.transform.position.y;

        this.position = pc.transform.position;

        this.movePower = pc.phys.movePower;
        //ADD STOREDVELOCITY CHECK HERE
        this.velocity = pc.phys.rb.velocity;

        this.Terrain = pc.phys.currentTerrain;

    }

    public override string ToString()
    {
        return "position: (" + xPos + ", " + yPos + "), power: " + movePower + ", velocity: " + velocity;
    } 



    
}
