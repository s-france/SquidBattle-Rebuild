using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsObj))]
public class PhysicsObjAnimation : MonoBehaviour
{
    public SpriteRenderer sr;
    [HideInInspector] public PhysicsObj phys;
    public PhysicsObjSpriteSet sprites;

    // Start is called before the first frame update
    public virtual void Start()
    {
        //sr = GetComponent<SpriteRenderer>();
        phys = GetComponent<PhysicsObj>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //default animation for physicsObj
        //really simple state-based animation
        sr.sprite = phys.CalcMovePrio(phys) switch
        {
            //standing still
            0 => sprites.idleSprite,
            //gliding
            1 => sprites.glidingSprite,
            //knockback
            2 => sprites.knockbackSprite,
            //moving (not knockback)
            3 => sprites.movingSprite,
            //
            _ => sprites.idleSprite,
        };

        if (phys.isHitStop)
        {
            sr.sprite = sprites.hitStopSprite;
        }

    }
    


}
