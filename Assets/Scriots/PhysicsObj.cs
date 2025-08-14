using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObj : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D solidCol;

    [SerializeField] AnimationCurve MoveCurve;

    [SerializeField] float maxMoveForce; 

    [SerializeField] AnimationCurve moveSpeedCurve;

    [SerializeField] AnimationCurve movePowerCurve;

    [SerializeField] AnimationCurve moveTimeCurve;

    [SerializeField] float maxMoveSpeed;

    [SerializeField] float maxMoveTime;

    [SerializeField] float maxMovePower;

     float moveSpeed = 0;

     float moveTime = 0;

     float movePower = 0;

    float moveTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        solidCol = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }



    void FixedUpdate()
    {

        MovementTick();

    }


    //runs in FixedUpdate()
    //processes movement
    void MovementTick()
    {
        if (moveTimer > moveTime)
        {
            moveTime = moveSpeed = movePower = 0;
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = rb.velocity.normalized * MoveCurve.Evaluate(moveTimer / moveTime) * moveSpeed;

            moveTimer += Time.fixedDeltaTime;
        }

        
    }

    //applys new move - sets movement variables
    void ApplyMove(float moveForce, Vector2 direction) // Assume moveForce is always between 0-1
    {
        moveTimer = 0;
        moveTime = maxMoveTime * moveTimeCurve.Evaluate(moveForce);
        movePower = maxMovePower * movePowerCurve.Evaluate(moveForce);
        moveSpeed = maxMoveSpeed * moveSpeedCurve.Evaluate(moveForce);
        rb.velocity = direction.normalized;
    }
}