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
        if(col.TryGetComponent<PhysicsObj>(out PhysicsObj otherObj))
        {
            otherObj.transform.position = transform.position;

            otherObj.ApplyMove(false, launchForce, transform.right);

        }


    }

}
