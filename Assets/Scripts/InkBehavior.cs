using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InkBehavior : ItemBehavior
{
    public GameObject InkPrefab;
    public float emissionRate;




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
            if (pc.phys.moveTimer % (pc.phys.stats.maxMoveTime/emissionRate) == 0)
            {
                SpawnInkObj(charge);
            }

            yield return GameManager.waitForFixedUpdate;
        }

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
        Vector3 size = Vector3.one; //size of ink cloud

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
