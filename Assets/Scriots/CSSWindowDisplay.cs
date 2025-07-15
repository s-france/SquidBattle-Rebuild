using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//sets UI display of a CSS PLayer Window
public class CSSWindowDisplay : MonoBehaviour
{
    PlayerManager pm;

    [HideInInspector] public int playerIdx = -1; //idx of player assigned to this window

    //UI element groups to be toggled/modified
    [SerializeField] Transform JoinLR; //Join L+R icons
    [SerializeField] Transform SwitchLR; //(color) Switch LR icons
    [SerializeField] Image PlayerIcon; //main player icon
    [SerializeField] Image LeftPlayerPreview; //L color preview
    [SerializeField] Image RightPlayerPreview; //R color preview

    // Start is called before the first frame update
    void Start()
    {
        //get PlayerManager
        pm =FindFirstObjectByType<PlayerManager>();

        //init to join state by default
        LeavePlayer();

    }

    // Update is called once per frame
    void Update()
    {

    }


    //resets display window to "waiting for player join" state
    public void LeavePlayer()
    {
        //show join icons
        JoinLR.gameObject.SetActive(true);

        //hide player interaction icons
        SwitchLR.gameObject.SetActive(false);
        PlayerIcon.gameObject.SetActive(false);

        //reset idx
        playerIdx = -1;
    }
}
