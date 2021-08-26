using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : RobotConnector2
{
    //드론을 연결하구 
    public int step; // 단계를 표현하는 

    bool isFlying; // 상태 확인 용

    Transform ViewBody;
    Rigidbody rb;

    int speed = 5;

    bool isSTbtning = false;
    float stbtntime = 0;
    bool collSenser = false;



    JOYPED joypad;
    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        Debug_tempBytes();//실시간 정보 받는 함수 업데이트문에 필수 
        //단계 별로 나누기 
        Co_START_STOP();// 1단계에서는 떳다 가라앉아다가만 가능 하기! 
        // 2단계 에서는 오른쪽 움직임만 되게 하기 
        // 3단계 에서는 왼쪽 위아래 회전 움직임 
        // 4단계 속도변화
        // 5단계 플립
        // 6단계 색바꾸기 
        // 7단계 뭐하지,암트은 
    }

    void NextStep(int next)
    {
        if (step > next)
        {
            step = next;
        }
    }

    //1단뎨 지나면 
    public void Co_START_STOP() //  버튼 누르면 함수 불렀다가 띠면 어떻게 표횬하지? 
    {
        // ------조이패드 ui 확인 -------//0번쨰가 st버튼 ! 
        if (isSTbtn)//버튼이 눌렸으면 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.CLICK, 0);
        }
        else // 버튼 손떼면 원래대로! 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.NORMAL, 0);
        }
        //=========실제 구동==========

        if (isSTbtn && !isSTbtning) //  isSTbtn 은 조이패드 입력갑으로 bool값 반환, 
        {
            stbtntime += Time.deltaTime;

            if (stbtntime >= 3.0f) //  버튼을 꾸욱 누르고 있으면 플립 작동 ! 
            {
                StopAllCoroutines();
                if (!isFlying)
                {
                    isFlying = true;
                    StartCoroutine(StartFly());
                }
                else
                {
                    StartCoroutine(StopFly());
                }
                isSTbtning = true;
            }
        }
        else // 떼었을 때
        {
            isSTbtning = false;

            stbtntime = 0;
        }
    }
    IEnumerator StartFly()
    {
        float now_Y = transform.position.y;
        while (now_Y + 3.0f >= transform.position.y)
        {
            transform.position += Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }

        NextStep(1);

        isFlying = true;
    }
    IEnumerator StopFly()
    {
        isFlying = false;
        ViewBody.localPosition = Vector3.zero;
        while (!collSenser)
        {
            transform.position -= Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
        NextStep(2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collSenser = true;
        print(collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        collSenser = false;
        print("bye" + collision.gameObject.name);
    }
}
