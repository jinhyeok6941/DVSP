using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : RobotConnector2
{
    //드론을 연결하구 
    private void Awake()
    {
        Connect_Manager();
    }
    void Start()
    {
        
    }

    void Update()
    {
        //단계 별로 나누기 
        // 1단계에서는 떳다 가라앉아다가만 가능 하기! 
        // 2단계 에서는 오른쪽 움직임만 되게 하기 
        // 3단계 에서는 왼쪽 위아래 회전 움직임 
        // 4단계 속도변화
        // 5단계 플립
        // 6단계 색바꾸기 
        // 7단계 뭐하지,암트은 
    }


}
