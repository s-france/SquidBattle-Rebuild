using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertCollider : MonoBehaviour
{
    private void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        Generate();
    }

    private void Generate()
    {
        PolygonCollider2D polyCol = GetComponent<PolygonCollider2D>();
        EdgeCollider2D edgeCol = GetComponent<EdgeCollider2D>();
        List<Vector2> points = new List<Vector2>(polyCol.points);
        points.Add(polyCol.points[0]);

        edgeCol.SetPoints(points);
    }
    


}

