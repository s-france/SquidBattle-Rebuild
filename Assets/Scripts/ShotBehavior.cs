using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehavior : ItemBehavior
{
    public GameObject ShotPrefab;
    
    public override int GetItemType()
    {
        return 1;
    }

    public override void UseItem(float charge)
    {
        //spawn projectile
        GameObject Shot = GameObject.Instantiate(ShotPrefab, pc.transform.position, pc.transform.rotation);

        //ProjectileObj ShotObj = Shot.GetComponent<ProjectileObj>();
        PhysicsObj ShotObj = Shot.GetComponent<PhysicsObj>();

        //need to Init Shot before we can call ApplyMove
        ShotObj.Start();

        //give user initial intangibiilty frames from the Shot
        ShotObj.SetPeerPriority(ShotObj.IntangiblePeerPrioTable, pc.phys, .5f);
        pc.phys.SetPeerPriority(pc.phys.IntangiblePeerPrioTable, ShotObj, .5f);

        //launch Shot projectile
        ShotObj.ApplyMove(false, charge, pc.aim_move.normalized);

        //apply recoil to player
        pc.phys.ApplyMove(false, .5f * charge, -pc.aim_move);

        DestroyItem();
    }
    
    public override void ChargeTick()
    {

    }



}
