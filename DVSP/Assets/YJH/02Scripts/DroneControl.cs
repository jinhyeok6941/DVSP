using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    public float upPower = 3;
    public float movePower = 1;
    Rigidbody rb;

    public bool isHobering = false;
    float swing = 0.1f;
    public Transform[] pos;

    float hoverY;
    float droneWeight;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //�����? ���� 
        droneWeight = rb.mass * Physics.gravity.magnitude; //  ������ ���� = ���� x �߷� ���ӵ�  / �� ũ��? 
    }
    void Update()
    {
        UpandDown();
        Ctrl4Way();
        //�¿�ȸ��

        //ȣ���� 
        Hovering();
        //�������?
        Balancing();

    }

    void UpandDown()
    {
        //�� �Ʒ�
        if (Input.GetKey(KeyCode.I))
        {
            for (int i = 0; i < pos.Length; i++)
            {
                rb.AddForceAtPosition(pos[i].up * upPower, pos[i].position); //  4���� ����
            }
        }
        else if (Input.GetKey(KeyCode.K))
        {
            for (int i = 0; i < pos.Length; i++)
            {
                rb.AddForceAtPosition(-pos[i].up * upPower * 0.1f, pos[i].position); //  4���� ����
            }
        }
    }//���Ʒ�

    void Ctrl4Way()
    {
        //�յ�
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddForceAtPosition(pos[0].up * movePower, pos[0].position);
            rb.AddForceAtPosition(pos[1].up * movePower, pos[1].position);

            rb.AddForceAtPosition(pos[2].up * (movePower - 0.1f), pos[2].position);
            rb.AddForceAtPosition(pos[3].up * (movePower - 0.1f), pos[3].position);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            rb.AddForceAtPosition(pos[2].up * movePower, pos[2].position);
            rb.AddForceAtPosition(pos[3].up * movePower, pos[3].position);

            rb.AddForceAtPosition(pos[1].up * (movePower - 0.1f), pos[0].position);
            rb.AddForceAtPosition(pos[0].up * (movePower - 0.1f), pos[1].position);
        }

        //�¿�
        if (Input.GetKeyDown(KeyCode.A))
        {
            rb.AddForceAtPosition(pos[1].up * movePower, pos[1].position);
            rb.AddForceAtPosition(pos[2].up * movePower, pos[2].position);

            rb.AddForceAtPosition(pos[3].up * (movePower - 0.1f), pos[3].position);
            rb.AddForceAtPosition(pos[0].up * (movePower - 0.1f), pos[0].position);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rb.AddForceAtPosition(pos[3].up * movePower, pos[3].position);
            rb.AddForceAtPosition(pos[0].up * movePower, pos[0].position);

            rb.AddForceAtPosition(pos[1].up * (movePower - 0.1f), pos[1].position);
            rb.AddForceAtPosition(pos[2].up * (movePower - 0.1f), pos[2].position);
        }
    }//�յ��¿�

    void Hovering()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isHobering)
            {
                //ȣ���� ���? 
                isHobering = !isHobering;
                hoverY = transform.position.y;// ��ư���������� ����               
                //StartCoroutine(Hover());
            }
            else
            {
                isHobering = !isHobering;
                //ȣ���� ���? ����
                //StopCoroutine(Hover());
            }
        }

        //ȣ���� ���? ����
        if (isHobering)
        {
            //ȣ���� ����϶���? ���? ������Ʈ �ռ����� ���� 
            if (hoverY > transform.position.y) // hover���� ������, �������� �ö󰡱�
            {
                print("up");
                for (int i = 0; i < pos.Length; i++)
                {
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.25f, pos[i].position);// 4���� ������ �߷� 4���� 1���� ������ �ö󰡰� ��. 
                }
            }
        }
        else
        {

        }



    }// ȣ���� ��ư 


    IEnumerator Hover()
    {
        while (isHobering)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    rb.AddForceAtPosition(pos[i].up * upPower, pos[i].position); //  4���� ����
                }
            }
            yield return new WaitForSeconds(swing);

        }
        print("end");
    }

    void Balancing()
    {
        if (transform.rotation.x >= 0 || transform.rotation.z >= 0)//�����? �������մٸ�
        {
            // �߽� ������ �����ִ� ���� �ش�.
            for (int i = 0; i < pos.Length; i++)
            {
                if (pos[i].position.y < transform.position.y - 0.1f) // �߽� ������ ���� ���� y ���� ������
                {
                    print("��������");
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.25f, pos[i].position); //���� �������� 
                }
            }
        }
    }
}