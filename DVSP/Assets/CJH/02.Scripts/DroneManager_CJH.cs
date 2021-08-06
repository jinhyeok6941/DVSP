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
            if(Input.GetKey(KeyCode.W))             // ��
            {
                 quad8.pitch = 10;
            }
            else if(Input.GetKey(KeyCode.S))        // ��
            {
                 quad8.pitch = -10;
            }
            else if(Input.GetKey(KeyCode.D))        // ��
            {
                 quad8.roll = 10;
            }
            else if(Input.GetKey(KeyCode.A))        // ��
            {
                quad8.roll  = -10;
            }
            else if(Input.GetKey(KeyCode.UpArrow))       // ��
            {
                quad8.throttle = 10;
            }
            else if(Input.GetKey(KeyCode.DownArrow))       // �Ʒ�
            {
                quad8.throttle = -10;
            }
            else if(Input.GetKey(KeyCode.RightArrow))    // �� ȸ��
            {
                quad8.yaw = 10;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))         // �� ȸ��
            {
                quad8.yaw = -10;
            }
            else if(Input.GetKeyDown(KeyCode.Space))    // take Off
            {
                takeoffPressed++;
            }
            else if(Input.GetKeyDown(KeyCode.L))         // ����
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
