using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    /*[SerializeField]*/ Slider wheel;
    PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        wheel = GetComponentInChildren<Slider>();

    }

    // Update is called once per frame
    void Update()
    {
        wheel.value = pc.stamina/pc.stats.maxStamina;

        if(wheel.value == 1)
        {
            wheel.gameObject.SetActive(false);
        } else
        {
            wheel.gameObject.SetActive(true);
        }

    }
}
