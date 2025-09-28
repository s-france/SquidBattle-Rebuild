using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTerrain : MonoBehaviour
{
    public GroundTerrainStats stats;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //send terrain enter message to col gameobject
        col.SendMessage("EnterTerrain", this);


    }

    void OnTriggerStay2D(Collider2D col)
    {

    }

    void OnTriggerExit2D(Collider2D col)
    {
        //send terrain exit message to col gameobject
        col.SendMessage("ExitTerrain", this);

    }



    public void TerrainEffect(PhysicsObj obj)
    {
        

    }
    



}
