using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate : MonoBehaviour
{
    //회전값
    Vector2 rot;
    public float rotSpeed = 200;

    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        rot.x += mx * rotSpeed * Time.deltaTime;
        rot.y += my * rotSpeed * Time.deltaTime;
        //90도 이상의 회전값을 가지지 않기
        //rotX = Mathf.Clamp(rotX, -90, 90);
        //local => 부모 중심으로 계산하겠다는 뜻
        transform.localEulerAngles = new Vector3(-rot.y, rot.x, 0);
    }
}
