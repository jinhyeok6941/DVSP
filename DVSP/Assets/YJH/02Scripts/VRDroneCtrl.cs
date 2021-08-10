Kusing System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : MonoBehaviour
{
    //��� ������ 
    public float speed = 10;

    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // ���
            //transform.position += Vector3.up * speed * Time.deltaTime; ;
            //StartFly(transform.position);
        }
        if (Input.GetKey(KeyCode.K))
        {
            //�ϰ�
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            //����
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S))
        {
            //����
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.A))
        {
            //����
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D))
        {
            //������
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }

    void StartFly()
    {
        Vector3 flyLine = transform.position + Vector3.up * 3;
        for (Vector3 pos = transform.position ; pos == flyLine; pos += Vector3.up * speed * Time.deltaTime)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            pos = transform.position;

        }
    }
}
