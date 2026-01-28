using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InkBehavior : ItemBehavior
{
    public GameObject InkPrefab;
    public float emissionRate;
    public float maxSize; //max size of ink cloud
    public AnimationCurve sizeCurve; //scaling of ink cloud size with charge
    public float sizeRandomness; //range of size variation
    public float directionRandomness; //range of launch direction variation


    public override int GetItemType()
    {
        return 2; //ink is 2
    }

    public override void UseItem(float charge)
    {
        
        //move player
        pc.phys.ApplyMove(false, Mathf.Clamp(charge / pc.stats.maxChargeTime, pc.stats.minCharge, 1), pc.aim_move);

        //Start Ink emission coroutine
        StartCoroutine(EmitInk(charge));

        //DestroyItem();
    }


    IEnumerator EmitInk(float charge)
    {
        //emit at least one initial ink cloud
        SpawnInkObj(charge);


        while (pc.phys.isMoving)
        {

            //emit at constant rate //FINISH: FIGURE OUT EMISSION RATE ROUNDING HERE!!!!
            //<=.015 is a little sus ideally we round things to fixedDeltaTime and use == 0
            if (pc.phys.moveTimer % (pc.phys.stats.maxMoveTime/emissionRate) <= .015f)
            {
                SpawnInkObj(charge);
            }

            yield return GameManager.waitForFixedUpdate;
        }
        
        //emit ink cloud on finish
        SpawnInkObj(charge);


        Debug.Log("Ink done!!");

        DestroyItem();


    }

    void SpawnInkObj(float charge)
    {
        GameObject Ink;
        PhysicsObj InkObj;

        //ADD THIS:  calc/randomize moveforce, size, and direction
        float moveForce = charge; //launch force of ink cloud
        Vector2 direction = -pc.phys.rb.velocity.normalized; //ink launch direction

        float sizeVal = Random.Range((maxSize * sizeCurve.Evaluate(charge / pc.stats.maxChargeTime)) - sizeRandomness, (maxSize * sizeCurve.Evaluate(charge / pc.stats.maxChargeTime)) + sizeRandomness);
        Vector3 size = new Vector3(sizeVal, sizeVal, 1); //size of ink cloud

        //spawn ink projectile
        Ink = GameObject.Instantiate(InkPrefab, pc.transform.position, pc.transform.rotation);
        InkObj = Ink.GetComponent<PhysicsObj>();

        //need to Init Ink before we can call ApplyMove
        InkObj.Start();

        //set random size
        InkObj.transform.localScale = size;


        //launch Ink projectile
        InkObj.ApplyMove(false, charge, direction);

    }
    
    public override void ChargeTick()
    {

    }
}
