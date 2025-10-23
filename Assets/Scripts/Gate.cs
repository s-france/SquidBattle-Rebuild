using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gate : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1);
    private static WaitForSeconds _waitForSeconds_5 = new WaitForSeconds(.25f);
    public bool allPlayersRoom; //specifies wether to summon all players to this gate when entered
    bool summoning = false;

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

    //unity message when player enters one of the gate's dashpads
    public void DashPadEntered(DashPad dp)
    {
        if (allPlayersRoom) { return; }

        if (dp == dashPad1)
        {
            StartCoroutine(DeactivateForSecs(dashPad2.gameObject, 1));
        }
        else if (dp == dashPad2)
        {
            StartCoroutine(DeactivateForSecs(dashPad1.gameObject, 1));
        }
    }

    //dashpad entered message for when a player enters
    public void DashPadEntered(PlayerController pc)
    {
        if (allPlayersRoom)
        {
            //PlayerManager.Instance.SummonPlayers(pc.transform.position, pc);


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

    public IEnumerator SummonPlayersThroughGate(DashPad dp, PlayerController first)
    {
        if (summoning) { yield break; }
        StopCoroutine(DeactivateForSecs(dashPad1.gameObject, 1));
        StopCoroutine(DeactivateForSecs(dashPad2.gameObject, 1));

        summoning = true;

        //disable gate dashpads
        dashPad1.gameObject.SetActive(false);
        dashPad2.gameObject.SetActive(false);

        //disable all players control/collision
        foreach (PlayerInput pi in PlayerManager.Instance.PlayerList)
        {
            if (pi.GetComponent<PlayerController>() != first)
            {
                pi.GetComponent<PhysicsObj>().DisableCollision();
                pi.DeactivateInput();
            }

        }


        //summon all players to dashpad
        PlayerManager.Instance.SummonPlayers(dp.transform.position, first);
        yield return _waitForSeconds1;


        //launch all players through dashpad
        foreach (PlayerInput pi in PlayerManager.Instance.PlayerList)
        {
            if (pi.GetComponent<PlayerController>() == first)
            {
                continue;
            }

            dp.gameObject.SetActive(true);
            dp.Launch(pi.GetComponent<PhysicsObj>());
            dp.gameObject.SetActive(false);

            //reenable player collision/input
            pi.GetComponent<PhysicsObj>().EnableCollision();

            yield return _waitForSeconds_5;
            pi.ActivateInput();
        }

        yield return _waitForSeconds_5;


        //reenable gate dashpads
        dashPad1.gameObject.SetActive(true);
        dashPad2.gameObject.SetActive(true);

        summoning = false;
    }

}
