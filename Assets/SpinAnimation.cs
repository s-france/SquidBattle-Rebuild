using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAnimation : MonoBehaviour
{
    public int minSpeed;
    public int maxSpeed;

    float speed;
    bool sign;


    // Start is called before the first frame update
    void Start()
    {
        sign = Random.value < 0.5f;
        speed = Random.Range(minSpeed,maxSpeed);

        if (sign)
        {
            speed *=-1;
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
