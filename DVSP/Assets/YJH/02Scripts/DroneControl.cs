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

    int rotPointCnt = 11;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        droneWeight = rb.mass * Physics.gravity.magnitude; 
    }
    void Update()
    {
        UpandDown();
        Ctrl4Way();

        Hovering();
        //Balancing();

    }

    void UpandDown()
    {
        if (Input.GetKey(KeyCode.I))
        {
            for (int i = 0; i < pos.Length; i++)
            {
                rb.AddForceAtPosition(pos[i].up * upPower, pos[i].position); 
            }
        }
        else if (Input.GetKey(KeyCode.K))
        {
            for (int i = 0; i < pos.Length; i++)
            {
                rb.AddForceAtPosition(-pos[i].up * upPower * 0.1f, pos[i].position); 
            }
        }
    }

    void Ctrl4Way()
    {
        //Forward 0,1 more power
        if (Input.GetKeyDown(KeyCode.W))
        {
            for (int i = 0; i < 20; i++)
            {
                rb.AddForceAtPosition(pos[0].up * movePower*3, pos[0].position);
                rb.AddForceAtPosition(pos[1].up * movePower*3, pos[1].position);

                rb.AddForceAtPosition(pos[2].up * droneWeight * 0.2f, pos[2].position);
                rb.AddForceAtPosition(pos[3].up * droneWeight * 0.2f, pos[3].position);
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < pos.Length; i++)// 4  golgoru 
                {
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.12f, pos[i].position);
                }
            }


        }
        //else if (Input.GetKeyUp(KeyCode.W))
        //{
        //    for (int i = 0; i < rotPointCnt - 1; i++)
        //    {
        //        rb.AddForceAtPosition(pos[0].up * droneWeight * 0.25f, pos[0].position);
        //        rb.AddForceAtPosition(pos[1].up * droneWeight * 0.25f, pos[1].position); ;

        //        rb.AddForceAtPosition(pos[2].up * movePower * 4, pos[2].position);
        //        rb.AddForceAtPosition(pos[3].up * movePower * 4, pos[3].position);
        //    }
        //}



        // Back 2,3  more power
        if (Input.GetKeyDown(KeyCode.S))
        {
            for (int i = 0; i < rotPointCnt; i++)
            {
                rb.AddForceAtPosition(pos[2].up * movePower * 3, pos[2].position);
                rb.AddForceAtPosition(pos[3].up * movePower * 3, pos[3].position);

                rb.AddForceAtPosition(pos[0].up * droneWeight * 0.2f, pos[0].position);
                rb.AddForceAtPosition(pos[1].up * droneWeight * 0.2f, pos[1].position);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForceAtPosition(pos[0].up * droneWeight * 0.15f, pos[0].position);
            rb.AddForceAtPosition(pos[1].up * droneWeight * 0.15f, pos[1].position);
            rb.AddForceAtPosition(pos[2].up * droneWeight * 0.15f, pos[2].position);
            rb.AddForceAtPosition(pos[3].up * droneWeight * 0.15f, pos[3].position);

        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            for (int i = 0; i < rotPointCnt - 1; i++)
            {
                rb.AddForceAtPosition(pos[2].up * droneWeight * 0.25f, pos[2].position);
                rb.AddForceAtPosition(pos[3].up * droneWeight * 0.25f, pos[3].position); ;

                rb.AddForceAtPosition(pos[0].up * movePower * 4, pos[0].position);
                rb.AddForceAtPosition(pos[1].up * movePower * 4, pos[1].position);
            }
        }

        ////�¿�
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    rb.AddForceAtPosition(pos[1].up * movePower, pos[1].position);
        //    rb.AddForceAtPosition(pos[2].up * movePower, pos[2].position);

        //    rb.AddForceAtPosition(pos[3].up * (movePower - 0.1f), pos[3].position);
        //    rb.AddForceAtPosition(pos[0].up * (movePower - 0.1f), pos[0].position);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    rb.AddForceAtPosition(pos[3].up * movePower, pos[3].position);
        //    rb.AddForceAtPosition(pos[0].up * movePower, pos[0].position);

        //    rb.AddForceAtPosition(pos[1].up * (movePower - 0.1f), pos[1].position);
        //    rb.AddForceAtPosition(pos[2].up * (movePower - 0.1f), pos[2].position);
        //}
    }

    void Hovering()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isHobering)
            {
                isHobering = !isHobering;
                hoverY = transform.position.y;
            }
            else
            {
                isHobering = !isHobering;
            }
        }

        //ȣ���� ���? ����
        if (isHobering)
        {
            //
            if (hoverY > transform.position.y)
            {
                print("up");
                for (int i = 0; i < pos.Length; i++)
                {
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.2f, pos[i].position);// 
                }
            }
        }
        else
        {

        }
    }



    void Balancing()
    {
        if (transform.rotation.x >= 0 || transform.rotation.z >= 0)//
        {
            //
            for (int i = 0; i < pos.Length; i++)
            {
                if (pos[i].position.y < transform.position.y - 0.1f) // 
                {
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.25f, pos[i].position); //
                }
            }
        }
    }
}