using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate : MonoBehaviour
{
    //ȸ����
    Vector2 rot;
    public float rotSpeed = 200;

    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        rot.x += mx * rotSpeed * Time.deltaTime;
        rot.y += my * rotSpeed * Time.deltaTime;
        //90�� �̻��� ȸ������ ������ �ʱ�
        //rotX = Mathf.Clamp(rotX, -90, 90);
        //local => �θ� �߽����� ����ϰڴٴ� ��
        transform.localEulerAngles = new Vector3(-rot.y, rot.x, 0);
    }
}
