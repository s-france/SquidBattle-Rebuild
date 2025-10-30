using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    [SerializeField] float launchForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<PhysicsObj>(out PhysicsObj otherObj))
        {
            //gameObject.SendMessageUpwards("DashPadEntered", this, SendMessageOptions.DontRequireReceiver);

            //send player data if a player is entering dashpad
            if (col.TryGetComponent<PlayerController>(out PlayerController pc))
            {
                gameObject.SendMessageUpwards("DashPadEntered", pc, SendMessageOptions.DontRequireReceiver);

                Gate g = GetComponentInParent<Gate>();

                if (g != null && g.allPlayersRoom)
                {
                    g.StartCoroutine(g.SummonPlayersThroughGate(this, pc));
                }

            }
            else
            {
                gameObject.SendMessageUpwards("DashPadEntered", this, SendMessageOptions.DontRequireReceiver);
            }


            Launch(otherObj);
        }


    }

    public void Launch(PhysicsObj obj)
    {
        obj.transform.position = transform.position;

        obj.ApplyMove(false, launchForce, transform.right);
        

    }
}
