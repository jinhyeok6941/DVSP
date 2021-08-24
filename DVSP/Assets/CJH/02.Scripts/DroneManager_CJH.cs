using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;





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
            else if(Input.GetKey(KeyCode.P))
            {
                byte[] packetBuffer = { 0x0A, 0x55, 0x04, 0x01, 0x10, 0x80, 0x42, 0x1D, 0xAF };
                //byte[] packetBuffer = { 0x0A, 0x55, 0x11, 0x02, 0x80, 0x10, 0x01, 0x00, 0xCD, 0xB6 };
                _serialPort.Write(packetBuffer, 0, packetBuffer.Length);
            }
            else
            {
                quad8.roll = 0;
                quad8.pitch = 0;
                quad8.throttle = 0;
                quad8.yaw = 0;
            }
            Debug_tempBytes();
            
            Angle_Move();
            //transform.position = new Vector3(motion.accX , motion.accY , motion.accZ);
        }
    }

    public override void Override_Test()
    {
        Debug.Log("sss");
    }
    
    Vector3 dir;

    void Angle_Move()
    {
       // motion.angleRoll 좌우 기울기. x 축
       // motion.anglePitch 전후 기울기. z 축
       // motion.angleYaw 좌우 회전도. rotation
       // 45도를 각도의 max치라고 가정한다.
       dir = new Vector3(motion.angleRoll , 0 , motion.anglePitch) * 0.01f;
       transform.Translate(dir * 10 * Time.deltaTime, Space.Self);
       transform.rotation = Quaternion.Euler(new Vector3(0, motion.angleYaw, 0));
       //transform.Rotate(0,1,0);
    }
}


