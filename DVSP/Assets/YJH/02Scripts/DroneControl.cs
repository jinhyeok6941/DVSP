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

    public int chack;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //
        droneWeight = rb.mass * Physics.gravity.magnitude; //  
    }
    void Update()
    {
        UpandDown();
        Ctrl4Way();
        //

        
        Hovering();
        
        // Balancing();

    }

    void UpandDown()
    {
        //
        if (Input.GetKey(KeyCode.I))
        {
            for (int i = 0; i < pos.Length; i++)
            {
                rb.AddForceAtPosition(pos[i].up * upPower, pos[i].position); //  
            }
        }
        //else if (Input.GetKey(KeyCode.K))
        //{
        //    for (int i = 0; i < pos.Length; i++)
        //    {
        //        rb.AddForceAtPosition(-pos[i].up * upPower * 0.1f, pos[i].position); //  
        //    }
        //}
    }

    void Ctrl4Way()
    {
        //forward 
        if (Input.GetKeyDown(KeyCode.W))//갈 방향으로 기울어지기
        {
            for (int i = 0; i < chack; i++)
            {
                rb.AddForceAtPosition(pos[0].up * movePower * 3, pos[0].position); //  big power
                rb.AddForceAtPosition(pos[1].up * movePower * 3, pos[1].position); // big power

                rb.AddForceAtPosition(pos[2].up * droneWeight * 0.2f, pos[2].position); // blance power
                rb.AddForceAtPosition(pos[3].up * droneWeight * 0.2f, pos[3].position); // blance power
            }
        }
        else if (Input.GetKey(KeyCode.W))//수평으로으로 이동
        {
            rb.AddForceAtPosition(pos[0].up * droneWeight * 0.15f, pos[0].position);
            rb.AddForceAtPosition(pos[1].up * droneWeight * 0.15f, pos[1].position);

            rb.AddForceAtPosition(pos[2].up * droneWeight * 0.15f, pos[2].position); // blance power
            rb.AddForceAtPosition(pos[3].up * droneWeight * 0.15f, pos[3].position); // blance power

            if(true)//너무 기울어지면 약한쪽이 힘을 좀 더써야할듯한데.. 45도 각도 유지행함. 
            { 

            }
        }
        else if (Input.GetKeyUp(KeyCode.W))// 균형잡기. 
        {
            for (int i = 0; i < chack -1.5f; i++)
            {
                rb.AddForceAtPosition(pos[0].up * droneWeight * 0.25f, pos[0].position);
                rb.AddForceAtPosition(pos[1].up * droneWeight * 0.25f, pos[1].position);

                rb.AddForceAtPosition(pos[2].up * movePower * 4, pos[2].position);
                rb.AddForceAtPosition(pos[3].up * movePower * 4, pos[3].position);

            }
        }


        ////  back
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    rb.AddForceAtPosition(pos[2].up * movePower, pos[2].position);//  big power
        //    rb.AddForceAtPosition(pos[3].up * movePower, pos[3].position);//  big power

        //    rb.AddForceAtPosition(pos[1].up * droneWeight * 0.25f, pos[0].position);// blance power
        //    rb.AddForceAtPosition(pos[0].up * droneWeight * 0.25f, pos[1].position);// blance power
        //}

        ////
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    rb.AddForceAtPosition(pos[1].up * movePower, pos[1].position);//  big power
        //    rb.AddForceAtPosition(pos[2].up * movePower, pos[2].position);//  big power

        //    rb.AddForceAtPosition(pos[3].up * droneWeight * 0.25f, pos[3].position);// blance power
        //    rb.AddForceAtPosition(pos[0].up * droneWeight * 0.25f, pos[0].position);// blance power
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    rb.AddForceAtPosition(pos[3].up * movePower, pos[3].position);//  big power
        //    rb.AddForceAtPosition(pos[0].up * movePower, pos[0].position);//  big power

        //    rb.AddForceAtPosition(pos[1].up * droneWeight * 0.25f, pos[1].position);// blance power
        //    rb.AddForceAtPosition(pos[2].up * droneWeight * 0.25f, pos[2].position);// blance power
        //}
    }//

    void Hovering()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isHobering)
            {
                //
                isHobering = !isHobering;
                hoverY = transform.position.y;// press butten timing . y            
            }
            else
            {
                isHobering = !isHobering;
            }
        }

        //
        if (isHobering)
        {
            //
            if (hoverY > transform.position.y) // now y , hover y 
            {
                print("up");
                for (int i = 0; i < pos.Length; i++)
                {
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.25f, pos[i].position);// 4 pos add power  
                }
            }
        }
        else
        {

        }



    }//


    void Balancing()// yet
    {
        if (transform.rotation.x >= 0 || transform.rotation.z >= 0)// 
        {
            // 
            for (int i = 0; i < pos.Length; i++)
            {
                if (pos[i].position.y < transform.position.y - 0.1f) // 중심보다 작으면
                {
                    print("asda");
                    rb.AddForceAtPosition(pos[i].up * droneWeight * 0.25f, pos[i].position); //
                }
            }
        }
    }
}