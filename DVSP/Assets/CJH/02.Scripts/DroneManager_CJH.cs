using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager_CJH : RobotConnector2
{
    public Transform[] pos;
    public Rigidbody rigid;
    float upPower = 10;
    //Awake ----------------------------------------------------------------------------------------------
    void Awake()
    {
        Connect_Manager();
    }

    // Update is called once per frame
    void Update()
    {
        if (_opened == true)
        { 
            if(Input.GetKey(KeyCode.W))             // 앞
            {
                 rigid.AddForceAtPosition(pos[3].up * upPower, pos[3].position);
                 rigid.AddForceAtPosition(pos[4].up * upPower, pos[4].position);
                 for(int i = 0 ; i < pos.Length ; i++)
                 {
                    rigid.AddForceAtPosition(pos[i].up * upPower * (i % 2), pos[i].position);
                 }
                 quad8.pitch = 0x46;
            }
            else if(Input.GetKey(KeyCode.S))        // 뒤
            {
                 quad8.pitch = -128;
            }
            else if(Input.GetKey(KeyCode.D))        // 좌
            {
                 quad8.roll = 0x46;
            }
            else if(Input.GetKey(KeyCode.A))        // 우
            {
                quad8.roll  = -128;
            }
            else if(Input.GetKey(KeyCode.UpArrow))       // 위
            {
                quad8.throttle = 0x46;
            }
            else if(Input.GetKey(KeyCode.DownArrow))       // 아래
            {
                quad8.throttle = -128;
            }
            else if(Input.GetKey(KeyCode.RightArrow))    // 우 회전
            {
                quad8.yaw = -128;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))         // 좌 회전
            {
                quad8.yaw = 0x46;
            }
            else if(Input.GetKeyDown(KeyCode.Space))    // take Off
            {
                takeoffPressed++;
            }
            else if(Input.GetKeyDown(KeyCode.L))         // 랜딩
            {
                landingPressed++;
            }
            else
            {
                for(int i = 0 ;i < pos.Length ; i++)
                {
                    rigid.AddForceAtPosition(pos[i].up * upPower, pos[i].position);
                }
                quad8.roll = 0;
                quad8.pitch = 0;
                quad8.throttle = 0;
                quad8.yaw = 0;
                //trimPressed++;
            }
            Debug_tempBytes();
        }
    }
}
