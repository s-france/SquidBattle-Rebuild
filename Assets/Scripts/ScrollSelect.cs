using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSelect : MonoBehaviour
{
    [SerializeField] Collider2D topCol;
    [SerializeField] Collider2D bottomCol;

    //[SerializeField] float scrollDistance; //transform.scale.y is the distance multiplier idk why but it works

    VerticalLayoutGroup content1; //contents of scroll select (on gameobject)
    VerticalLayoutGroup content2; //copy of content1 for wraparound scrolling (instantiated at runtime)

    bool topContent2 = false; //true = content2 on top //false = content1 on top
    int selected = 0; //selected element relative to edges
    Vector2 position; //y position of scrolling list
    int edge;


    // Start is called before the first frame update
    void Start()
    {
        content1 = GetComponentInChildren<VerticalLayoutGroup>();

        Vector3 pos = new Vector3(content1.transform.position.x, content1.transform.position.y - (transform.localScale.y * content1.transform.childCount));

        content2 = GameObject.Instantiate(content1.gameObject, pos, Quaternion.identity, content1.transform.parent).GetComponent<VerticalLayoutGroup>();

        //edge position = amount of content * 2
        edge = (content1.transform.childCount * 2) - 1;

        position = content1.transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        //scrolling animation (final float is speed of scroll)
        content1.transform.parent.position = Vector2.MoveTowards(content1.transform.parent.position, position, .01f);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {

        if (col.otherCollider == topCol)
        {
            Scroll(-1);
        }
        else if (col.otherCollider == bottomCol)
        {
            Scroll(1);
        }
    }

    public void Scroll(int direction)
    {

        //loop if on an edge
        if (selected == 0 && direction < 0) //top edge
        {
            //put outer content on top
            if (!topContent2)
            {
                Debug.Log("content2 on top!");

                content2.transform.position = new Vector3(content1.transform.position.x, content1.transform.position.y + (transform.localScale.y * content1.transform.childCount), 0);

                topContent2 = true;
            }
            else
            {
                Debug.Log("content1 on top!");
                content1.transform.position = new Vector3(content2.transform.position.x, content2.transform.position.y + (transform.localScale.y * content2.transform.childCount), 0);
                topContent2 = false;
            }

            selected = content1.transform.childCount - 1; //now at bottom edge of top content
        }
        else if (selected == edge && direction > 0) //bottom edge
        {
            if (topContent2)
            {
                content2.transform.position = new Vector3(content1.transform.position.x, content1.transform.position.y - (transform.localScale.y * content1.transform.childCount), 0);
                topContent2 = false;
            }
            else
            {
                content1.transform.position = new Vector3(content2.transform.position.x, content2.transform.position.y - (transform.localScale.y * content2.transform.childCount), 0);
                topContent2 = true;
            }

            selected = content1.transform.childCount; //now at top edge of bottom content

        }
        else //no loop
        {
            selected += direction;
        }


        //scroll in specified direction

        position += transform.localScale.y * direction * Vector2.up;

        //content1.transform.parent.position = (Vector2)content1.transform.parent.position + (transform.localScale.y * direction * Vector2.up);
    }


}
