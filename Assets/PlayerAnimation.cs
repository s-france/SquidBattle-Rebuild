using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    PlayerController pc;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        AnimationTick();
    }

    void AnimationTick()
    {
        if (pc.phys.isHitstop)
        {

        }

        //rotate
        if (!pc.isKnockback && !pc.phys.isMoving)
        {
            RotatePlayer(pc.aim_move);
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
