using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//helper script for sending collision events to colliders' parents
public class CollisionRelay : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        //relay OnCollisionEnter2D message to parents
        transform.parent.SendMessageUpwards("OnCollisionEnter2D", col);
    }

}
