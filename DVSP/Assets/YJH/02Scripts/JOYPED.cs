using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JOYPED : RobotConnector2
{
    public Transform joystickL;
    public Transform joystickR;


    private void Awake()
    {
        Connect_Manager();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Debug_tempBytes();//실시간 정보 받는 함수 업데이트문에 필수 

        joystickL.localPosition = new Vector3(L_x * 0.003f , L_y * 0.003f, -1);
        joystickR.localPosition = new Vector3(R_x * 0.003f, R_y * 0.003f, -1);
    }


}
