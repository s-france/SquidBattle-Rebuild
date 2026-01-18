using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerContainer : MonoBehaviour
{
    List<PlayerController> Contents;
    public UnityEvent<PlayerController> playerEnterEvent;


    // Start is called before the first frame update
    void Start()
    {
        Contents = new List<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<PlayerController>(out PlayerController pc) && !Contents.Contains(pc))
        {
            Contents.Add(pc);

            
            playerEnterEvent.Invoke(pc);
            
        }
        
    }
}
