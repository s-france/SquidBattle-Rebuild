using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//a switch that can be flipped by contact with a physicsobj
public class Switch : MonoBehaviour
{
    public UnityEvent<PhysicsObj, bool> onSwitchHitEvent;
    public bool isOn;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            //flip on/off
            isOn = !isOn;

            onSwitchHitEvent.Invoke(obj, isOn);
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            //flip on/off
            isOn = !isOn;

            onSwitchHitEvent.Invoke(obj, isOn);
        }
    }



}
