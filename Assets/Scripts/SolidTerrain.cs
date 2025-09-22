using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//specifications and behavior of a solid piece of terrain (wall)
public class SolidTerrain : MonoBehaviour
{
    public SolidTerrainStats stats;

    [HideInInspector] public float hp; //current health of terrain

    // Start is called before the first frame update
    void Start()
    {
        //init to max hp
        hp = stats.maxHP;

    }

    /*
    // Update is called once per frame
    void Update()
    {

    }
    */



    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            //IF NECESSARY IDK MIGHT ALREADY HAPPEN:
            //redirect physobj movement direction

            //modify obj movement based on terrain stats
            obj.ModifyMove(obj.isKnockback, Vector2.zero, stats.timeMod, stats.speedMod, stats.powerMod);

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
