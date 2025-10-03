using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindBehavior : ItemBehavior
{
    public override int GetItemType()
    {
        return 0;
    }

    public override void UseItem(float charge)
    {
        pc.StartCoroutine(pc.Rewind(charge));
        DestroyItem();
    }
    
    public override void ChargeTick()
    {

    }
}
