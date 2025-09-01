using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//works in tandem with a PhysicsObj to process collision with other phyhsics objs
public class PhysicsTriggerHB : MonoBehaviour
{
    PhysicsObj PhysO;
    Collider2D hb;

    [SerializeField] float pushOutPower; //amount of force this object should push other objs out


    List<Collider2D> Collisions; //list of all colliders currently touching hb

    // Start is called before the first frame update
    void Start()
    {
        PhysO = GetComponentInParent<PhysicsObj>();
        hb = GetComponent<Collider2D>();

        //init collisions list
        Collisions = new List<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        //update collisions list
        if (!Collisions.Contains(col))
        {
            Collisions.Add(col);

            //physics process collision
        }



    }


    void OnTriggerExit2D(Collider2D col)
    {
        Collisions.Remove(col);
    }


    void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent<PhysicsTriggerHB>(out PhysicsTriggerHB othercol))
        {
            //push out overlapping triggerHBs
            Vector2 away = (col.transform.position - transform.position).normalized;

            col.transform.parent.position = (Vector2)col.transform.parent.position + (pushOutPower * Time.fixedDeltaTime * away);
        }


        
    }
}
