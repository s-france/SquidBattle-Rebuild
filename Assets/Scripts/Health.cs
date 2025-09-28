using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generic class for objects with hp
public class Health : MonoBehaviour
{
    [HideInInspector] public float hp;
    public float maxHP;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void HealHP(float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHP);
    }

    public void DamageHP(float amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            //FINISH THIS: hp == 0 dying message
            //SendMessage("Kill");

        }
    }

}
