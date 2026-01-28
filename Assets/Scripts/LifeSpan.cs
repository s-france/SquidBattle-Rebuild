using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for despawning objects after a certain amount of time
public class LifeSpan : MonoBehaviour
{
    public float lifeSpan; //how long this object should remain active
    [HideInInspector] public float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= lifeSpan)
        {
            SendMessage("StopAllCoroutines");
            Destroy(gameObject);
        }
        
    }
}
