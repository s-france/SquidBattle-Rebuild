using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimation : PhysicsObjAnimation
{
    PlayerController pc;
    Rigidbody2D rb;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

    }

    public override void Update()
    {
        base.Update();

        AnimationTick();
    }

    void AnimationTick()
    {


        //aim rotate
        if (!pc.phys.isKnockback && !pc.phys.isMoving && !pc.isRewind)
        {
            RotatePlayer(pc.aim_move);
        }
        //move rotate
        if (pc.phys.isMoving && !pc.phys.isKnockback)
        {
            RotatePlayer(rb.velocity.normalized);
        }

        //charging sprite
        if (pc.charging)
        {
            sr.sprite = ((PlayerSpriteSet)sprites).chargingSprite;

        }

    }



    //makes player look in direction
    public void RotatePlayer(Vector2 dir)
    {
        rb.angularVelocity = 0;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

}
