using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager_CJH : RobotConnector2
{
    // Awake ----------------------------------------------------------------------------------------------
    void Awake()
    {
        Connect_Manager();
    }

    // Update is called once per frame
    void Update()
    {
        if (_opened == true)
        {
            //byte[] tempBytes = Read();
            if(Input.GetKey(KeyCode.W))             // 앞
            {
                 quad8.pitch = 10;
            }
            else if(Input.GetKey(KeyCode.S))        // 뒤
            {
                 quad8.pitch = -10;
            }
            else if(Input.GetKey(KeyCode.D))        // 좌
            {
                 quad8.roll = 10;
            }
            else if(Input.GetKey(KeyCode.A))        // 우
            {
                quad8.roll  = -10;
            }
            else if(Input.GetKey(KeyCode.UpArrow))       // 위
            {
                quad8.throttle = 10;
            }
            else if(Input.GetKey(KeyCode.DownArrow))       // 아래
            {
                quad8.throttle = -10;
            }
            else if(Input.GetKey(KeyCode.RightArrow))    // 우 회전
            {
                quad8.yaw = 10;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))         // 좌 회전
            {
                quad8.yaw = -10;
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
                quad8.roll = 0;
                quad8.pitch = 0;
                quad8.throttle = 0;
                quad8.yaw = 0;
                //trimPressed++;
            }
        }
    }
}
