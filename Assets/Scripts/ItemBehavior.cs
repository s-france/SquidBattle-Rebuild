using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public Transform FollowPoint;
    [HideInInspector] public Collider2D pickupTrigger;

    float followSpeed = 3;
    float accelRate = 3.5f;

    ColorSet Colors;


    // Start is called before the first frame update
    void Start()
    {
        pickupTrigger = GetComponent<Collider2D>();

    }

    void FixedUpdate()
    {
        if (pc != null)
        {
            ChargeTick();
            Follow();
        }
    }

    public virtual int GetItemType()
    {

        return -1;
    }

    public virtual void UseItem(float charge)
    {
        Debug.Log("ERROR: no item type assigned!");
        DestroyItem();
    }



    public virtual void DestroyItem()
    {
        GameObject.Destroy(gameObject);
    }


    public virtual void ChargeTick()
    {

    }


    //ItemObj stuff
    public void AssignPlayer(PlayerController p)
    {
        pc = p;
        pc.ItemInventory.Add(this);

        pickupTrigger.enabled = false; //disable future pickup collision

        int index = pc.ItemInventory.IndexOf(this);
        FollowPoint = pc.transform.Find("FollowPoints").GetChild(index);

    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (pc == null)
        {
            if (col.TryGetComponent<PlayerController>(out PlayerController p))
            {
                if (p.ItemInventory.Count < p.itemInventorySize)
                {
                    AssignPlayer(p);
                }
            }
        }
    }


    public void Follow()
    {
        float acceleration;
        acceleration = accelRate * (transform.position - FollowPoint.position).magnitude;
        transform.position = Vector2.MoveTowards(transform.position, FollowPoint.position, followSpeed * acceleration * Time.deltaTime);
    }

}
