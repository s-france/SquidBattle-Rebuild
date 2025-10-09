using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Important organizational level data for a single player
public class PlayerData : MonoBehaviour
{
    public PlayerInput pi; //use pi.playerIndex for true indexing!

    public SpriteRenderer BodySprite;

    //[HideInInspector] public int trueIdx; //PlayerManager ID -i.e. order controller was connected
    [HideInInspector] public int inGameIdx = -1; //Player Number -by order of CSS joins: P1,P2, etc.
    [HideInInspector] public int teamIdx = -1; //currently assigned team;
    [HideInInspector] public Team Team;

    [HideInInspector] public int colorIdx; //ID associated with color -used in PlayerManager color lookup
    [HideInInspector] public Color color; //equipped color




    // Start is called before the first frame update
    void Start()
    {
        pi = GetComponent<PlayerInput>();

    }

    // Update is called once per frame
    void Update()
    {

    }



    //sets all player assets to current color
    public void SetColor()
    {
        BodySprite.color = PlayerManager.Instance.PlayerColors.Colors[colorIdx];

    }
}
