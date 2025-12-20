using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//a switch that can be flipped by contact with a physicsobj
public class Switch : MonoBehaviour
{
    public UnityEvent<PhysicsObj, bool> onSwitchHitEvent;
    public bool isOn;

    Transform OnDisplay; // = transform.GetChild(0);
    Transform OffDisplay; // = transform.GetChild(1);


    // Start is called before the first frame update
    void Start()
    {
        OnDisplay = transform.GetChild(0);
        OffDisplay = transform.GetChild(1);

        ToggleSwitch();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            Debug.Log("switch collided");

            //flip on/off
            isOn = !isOn;

            onSwitchHitEvent.Invoke(obj, isOn);

            ToggleSwitch();
        }
    }

    
    /*
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<PhysicsObj>(out PhysicsObj obj))
        {
            Debug.Log("switch trigger enter");

            //flip on/off
            isOn = !isOn;

            onSwitchHitEvent.Invoke(obj, isOn);

            ToggleSwitch();
        }
    }
    */


    //updates switch display
    public void ToggleSwitch()
    {
        if (isOn)
        {
            OnDisplay.gameObject.SetActive(true);
            OffDisplay.gameObject.SetActive(false);
        }
        else
        {
            OnDisplay.gameObject.SetActive(false);
            OffDisplay.gameObject.SetActive(true);
        }
    }



}
