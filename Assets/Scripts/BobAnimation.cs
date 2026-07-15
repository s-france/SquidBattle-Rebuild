using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple bobbing up + down animation
public class BobAnimation : MonoBehaviour
{
    public AnimationCurve loop;
    public float bobDistance;
    public float bobSpeed;

    float tick = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        tick += (Time.fixedDeltaTime * bobSpeed);
        /*
        if (tick >=1)
        {
            tick = 0;
        }
        else
        {
            tick += (Time.fixedDeltaTime * bobSpeed);
        }
        */

        BobLoop();
    }

    void BobLoop()
    {
        transform.localPosition = bobDistance * Vector3.up * loop.Evaluate(tick);

        /*
        if ((Vector2)transform.position == target)
        {
            bobDistance = -bobDistance;
        }

        target = originalPos + bobDistance;
        
        transform.position = Vector2.MoveTowards(transform.position, target, bobSpeed);
        */

    }

}
