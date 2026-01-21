using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerContainer : MonoBehaviour
{
    public int capacity;
    [HideInInspector] public List<PlayerController> Contents;
    public UnityEvent<PlayerController> playerEnterEvent;
    public UnityEvent<PlayerController> playerExitEvent;

    public string actionMap;


    // Start is called before the first frame update
    void Start()
    {
        Contents = new List<PlayerController>(capacity);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<PlayerController>(out PlayerController pc) && !Contents.Contains(pc) && Contents.Count < capacity)
        {
            EnterContainer(pc);
        }
    }

    void EnterContainer(PlayerController pc)
    {
        Contents.Add(pc);
        pc.Container = this;

        //put player inside container
        pc.phys.CancelMove();
        pc.transform.position = transform.position;
        pc.phys.solidCol.enabled = false;
        pc.phys.triggerCol.enabled = false;

        //set player inputs to container specifications
        pc.pi.SwitchCurrentActionMap(actionMap);


        SendMessage("OnPlayerEnter", pc);
        playerEnterEvent.Invoke(pc);
    }

    void ExitContainer(PlayerController pc)
    {
        if(Contents.Contains(pc))
        {
            Contents.Remove(pc);

            pc.Container = null;

            pc.phys.solidCol.enabled = true;
            pc.phys.triggerCol.enabled = true;

            //set player inputs to container specifications
            pc.pi.SwitchCurrentActionMap("GamePlay");


            SendMessage("OnPlayerExit", pc);
            playerExitEvent.Invoke(pc);

        }
    }
}
