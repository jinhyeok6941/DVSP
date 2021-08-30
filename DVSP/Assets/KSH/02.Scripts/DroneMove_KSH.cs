using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMove_KSH : MonoBehaviour
{
    public float moveSpeed = 5;
    //��� �ϰ� �ӵ�
    private float upSpeed = 20;
    private float rotSpeed = 150;

    void Update()
    {
        DroneMoveCtrl();
    }

    void DroneMoveCtrl()
    {
        //����, ����, ��, ��
        if (Input.GetKey(KeyCode.UpArrow))  transform.position += transform.forward * Time.deltaTime * moveSpeed;
        if (Input.GetKey(KeyCode.DownArrow))  transform.position += -transform.forward * Time.deltaTime * moveSpeed;
        if (Input.GetKey(KeyCode.LeftArrow))  transform.position += -transform.right * Time.deltaTime * moveSpeed;
        if (Input.GetKey(KeyCode.RightArrow))  transform.position += transform.right * Time.deltaTime * moveSpeed;


        //���, �ϰ�, ��, �� ���� ���� 
        if (Input.GetKey(KeyCode.W))   transform.position += transform.up * Time.deltaTime * moveSpeed;
        if (Input.GetKey(KeyCode.S))   transform.position += -transform.up * Time.deltaTime * moveSpeed;
        if (Input.GetKey(KeyCode.A))   transform.eulerAngles += -transform.up * Time.deltaTime * rotSpeed;
        if (Input.GetKey(KeyCode.D))   transform.eulerAngles += transform.up * Time.deltaTime * rotSpeed;
    }
}
