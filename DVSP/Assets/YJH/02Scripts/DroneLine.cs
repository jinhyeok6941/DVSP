using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLine : MonoBehaviour
{
    public GameObject linePrefab;
    GameObject nowLine;
    LineRenderer wLineRenderer;

    List<Vector3> points = new List<Vector3>();
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            nowLine = Instantiate(linePrefab,transform.position,Quaternion.identity);
            wLineRenderer = nowLine.GetComponent<LineRenderer>();

            points.Add(transform.position);
            wLineRenderer.positionCount = 1;
            wLineRenderer.SetPosition(0,points[0]);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            if (Vector3.Distance(points[points.Count - 1], transform.position) > 0.01f)
            {
                points.Add(transform.position);
                wLineRenderer.positionCount++;
                wLineRenderer.SetPosition(wLineRenderer.positionCount - 1, transform.position);
            }
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            points.Clear();
            nowLine = null;
            wLineRenderer = null;
        }
    }
}
