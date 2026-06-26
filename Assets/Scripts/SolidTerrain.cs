using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//specifications and behavior of a solid piece of terrain (wall)
public class SolidTerrain : MonoBehaviour
{
    public SolidTerrainStats stats;

    [HideInInspector] public float hp; //current health of terrain

    List<Collider2D> Collisions; //list of all colliders currently touching this

    // Start is called before the first frame update
    void Start()
    {
        Collisions = new List<Collider2D>();

        //init to max hp
        hp = stats.maxHP;

    }




    void OnCollisionEnter2D(Collision2D col)
    {
        /*
        if (!Collisions.Contains(col.collider))
        {
            Collisions.Add(col.collider);
        } else
        {
            return;
        }
        */

        if (col.gameObject.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            //small amount of hitstop on collision
            obj.ApplyHitStop(0, 2*Time.fixedDeltaTime);

            //modify obj movement based on terrain stats
            obj.ModifyMove(obj.isKnockback, Vector2.zero, stats.timeMod, stats.speedMod, stats.powerMod);

            //calc + apply damage received
            if (obj.dealsDamage)
            {
                Vector2 normal = col.GetContact(0).normal.normalized;

                //calc directness of impact
                //1 = direct hit
                //0 = indirect/parallel hit
                float directness = Vector2.Angle(col.GetContact(0).normal.normalized, obj.rb.velocity) / 180;

                //calc terrain damage
                float strength = directness * obj.movePower;

                //if strength exceeds armor value -> deal terrain damage
                if (strength > stats.armor)
                {
                    hp -= strength;
                }

                //destroy if hp depleted
                if (hp <= 0)
                {
                    Destroy(gameObject);
                }
            }


            

        }
        

    }

    void OnCollisionExit2D(Collision2D col)
    {
        //Collisions.Remove(col.collider);
        
    }


}
