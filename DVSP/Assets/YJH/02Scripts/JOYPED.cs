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
        Debug_tempBytes();//�ǽð� ���� �޴� �Լ� ������Ʈ���� �ʼ� 

        joystickL.localPosition = new Vector3(L_x * 0.003f , L_y * 0.003f, -1);
        joystickR.localPosition = new Vector3(R_x * 0.003f, R_y * 0.003f, -1);
    }


}
