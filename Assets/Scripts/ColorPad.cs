using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPad : MonoBehaviour
{
    public int colorID;
    SpriteRenderer sr;

    bool isActive = true;

    Color disabledColor;

    // Start is called before the first frame update
    void Start()
    {
        disabledColor = new Color(.5f, .5f, .5f, .5f);

        sr = GetComponent<SpriteRenderer>();
        Activate();

        PlayerManager.colorChangedEvent.AddListener(OnColorChangeEvent);

        //initialize based on active player colors
        OnColorChangeEvent(null, colorID);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<PlayerData>(out PlayerData pd) && isActive)
        {
            PlayerManager.colorChangedEvent.Invoke(pd, colorID);
        }

    }

    void Activate()
    {
        sr.color = PlayerManager.Instance.PlayerColors.Colors[colorID];
        isActive = true;
        
    }
    void Deactivate()
    {
        isActive = false;
        sr.color = disabledColor;
    }

    void OnColorChangeEvent(PlayerData pd, int color)
    {

        if (PlayerManager.Instance.TakenColors.Contains(colorID))
        {
            Deactivate();
        }
        else
        {
            Activate();
        }

           
        

    }
}
