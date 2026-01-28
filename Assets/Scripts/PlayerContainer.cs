using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerContainer : MonoBehaviour
{
    public int capacity; //how many players can occupy container at once
    public Vector2 EjectStat = Vector2.down; //vector dictates exit force + direction
    //public float ejectRandomness; //randomization factor for eject direction
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

            //move this above contents.remove^^ if eject doesn't work...
            pc.phys.solidCol.enabled = true;
            pc.phys.triggerCol.enabled = true;

            //set player input action map back to gameplay mode
            pc.pi.SwitchCurrentActionMap("GamePlay");


            SendMessage("OnPlayerExit", pc);
            playerExitEvent.Invoke(pc);

        }
    }

    //handles behavior for ejecting players from container
    public virtual void Eject(PlayerController pc)
    {
        //eject downwards by default
        pc.phys.ApplyMove(true, EjectStat.magnitude, EjectStat.normalized);
        
    }


}
