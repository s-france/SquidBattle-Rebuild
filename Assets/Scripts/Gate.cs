using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public DashPad dashPad1;
    public DashPad dashPad2;

    WaitForFixedUpdate fuWait;

    // Start is called before the first frame update
    void Start()
    {
        fuWait = new WaitForFixedUpdate();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DashPadEntered(DashPad dp)
    {
        if (dp == dashPad1)
        {
            StartCoroutine(DeactivateForSecs(dashPad2.gameObject, 1));
        }
        else if (dp == dashPad2)
        {
            StartCoroutine(DeactivateForSecs(dashPad1.gameObject, 1));
        }


    }

    //deactivates a gameobject for an amount of time
    public IEnumerator DeactivateForSecs(GameObject obj, float time)
    {
        obj.SetActive(false);

        float timer = 0;
        while (timer < time)
        {
            //waiting for time

            timer += Time.fixedDeltaTime;
            yield return fuWait;
        }

        obj.SetActive(true);

    }

}
