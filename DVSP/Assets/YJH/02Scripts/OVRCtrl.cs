using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRCtrl : MonoBehaviour
{
    public float speed = 10;
    public float rotSpeed = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        L_MOVE();
        R_MOVE();
    }

    void L_MOVE()
    {
        Vector2 pedL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        transform.Translate(Vector3.up * pedL.y * Time.deltaTime); //상하 움직임  

        transform.Rotate(Vector3.up * pedL.x, rotSpeed);//좌우전환
    }

    void R_MOVE()
    {
        Vector2 pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        Vector3 dirR = new Vector3(pedR.x, 0, pedR.y);

        transform.Translate(dirR * speed * Time.deltaTime); // 전후좌우 움직임
    }
}
