using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : RobotConnector2
{
    public JOYPED joypad;
    //드론을 연결하구 
    public int step; // 단계를 표현하는 

    bool isFlying; // 상태 확인 용

    Transform ViewBody;
    Rigidbody rb;

    int speed = 5;
    int spCount = 0;

    bool isSTbtning = false;
    float stbtntime = 0;

    bool isFliping = false;
    float flipTime;

    bool collSenser = false;

    public Space flymode = Space.Self;
    Vector3 rotate_aix;

    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.WakeUp(); // 리지드 바디 계 속 꺠워주기.  
        Debug_tempBytes();//실시간 정보 받는 함수 업데이트문에 필수 
        //단계 별로 나누기 
        Co_START_STOP();// 1단계에서는 떳다 가라앉아다가만 가능 하기! // 0,1,2 step
            // ^1 4단계 속도변화 // 9,10,11 스텝 
        FLIP_Motion(); // 5단계 플립 // 12,13,14 step
            //^! 6단계 색변화 // 15 , 16, 17 
        if (isFlying && !isFlip) /// 비행중이고 플립상태가 아닐떄! 
        {
            R_JoyStick();  // 2단계 에서는 오른쪽 움직임만 되게 하기 // 3, 4, 5 Step
            L_JoyStick(); // 3단계 에서는 왼쪽 위아래 회전 움직임  // 6 , 7, 8 step
        }
    }

    void NextStep(int next)
    {
        if (step < next)
        {
            step = next;
        }
    }

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
            if (isFlying && stbtntime <= 1    &&  step >= 9) //비행중이고 1이내로 누르고 있었으면 속도향상       // 9스텝 이상떄만 작동
            {
                speed /= (spCount % 3 + 1); //원래 숫자로만들고 
                spCount++;
                speed *= (spCount % 3 + 1); //  하나 더한 값만큼 곱해주기 !

                if (step == 9)
                {
                    StartCoroutine(WaitTime(3.0f,2.0f, 10)); 
                    //10 스텝으로 넘겨 놓고 // 3초 뒤에 11스텝 클리어 메세지 // 2초뒤 12스텝 (다음 스테이지로 !)
                }
            }
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

        isFlying = true;

        NextStep(1); //  1번은 Stop step
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

        NextStep(2); // 2번은 클리어 메시지
        StartCoroutine(WaitTime(2.0f,3)); // 2초 기다리고 3번 step로 넘어가기! 

        if (step == 15) //  15스텝일떄 또다시 착륙하면 16스텝으로 !
        {
            NextStep(16);
        }
    }

    void FLIP_Motion()
    {
        if (step < 12) return;

        // ------조이패드 ui 확인 -------
        if (isFlip)//버튼이 눌렸으면 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.CLICK, 1);
        }
        else // 버튼 손떼면 원래대로! 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.NORMAL, 1);
        }

        //=========실제 구동==========

        if (isFlip && !isFliping) //  isFlip은 조이패드 입력갑으로 bool값 반환, 
        {
            NextStep(13); 
            flipTime += Time.deltaTime;

            if (flipTime >= 3.0f && isFlying) // 비행중일 떄 버튼을 꾸욱 누르고 있으면 플립 작동 ! 
            {
                StopAllCoroutines();
                StartCoroutine(Flip_H());
                StartCoroutine(Flip_V());
                isFliping = true;
            }
        }
        else
        {            
            isFliping = false;
            if (flipTime < 1 && step > 16  && !isFlying) // 비행중아니고 16스텝 이상부터 1초 이하의 경우에만 작동 ! 
            {/// 랜덤 색 변화 코딩 
                GameObject led = ViewBody.GetChild(0).gameObject;
                MeshRenderer mr = led.GetComponent<MeshRenderer>();
                float r = Random.Range(0.0f, 1.0f);
                float g = Random.Range(0.0f, 1.0f);
                float b = Random.Range(0.0f, 1.0f);
                mr.material.color = new Color(r, g, b);
                NextStep(17); // 17 클리에 메세지 보여주고 끝! !
                print(led.name);
            }
            flipTime = 0;
        }


    }
    IEnumerator Flip_V()
    {
        float currtime = 0;

        while (currtime <= 2.0f)
        {
            currtime += Time.deltaTime;
            if (currtime <= 1.0f)//1초 전에는 올라가구 
            {
                transform.position += Vector3.up * 0.02f;
            }
            else// 후 에는 내려가고 
            {
                transform.position -= Vector3.up * 0.02f;
            }

            yield return new WaitForEndOfFrame();
        }
        NextStep(14); // 14step 기다리고 다음으로! 
        StartCoroutine(WaitTime(2.0f, 15)); // 15단계로! 
    }
    IEnumerator Flip_H()// 수평방향 움직임 & 움직이는 방향 회전 
    {
        //isFlip = true;
        float currtime = 0;
        Vector3 flip_dir = new Vector3(R_x, 0, R_y);
        flip_dir.Normalize();
        ViewBody.rotation = transform.rotation;//일단 보여지는 몸체 초기화

        Vector3 rotaix = Vector3.Cross(flip_dir, Vector3.up);
        while (currtime <= 2.0f)
        {
            currtime += Time.deltaTime;

            if (currtime <= 0.5f) // 0 ~ 0.5초 사이
            {
                // dir 방향으로 이동
                transform.position += flip_dir * 0.01f;
            }
            else if (currtime > 0.5f && currtime <= 1.5f)
            {
                // - dir 방향으로 이동 
                transform.position -= flip_dir * 0.01f;
            }
            else // 1.5 초 이후  
            {
                //dir 방향으로 이동 
                transform.position += flip_dir * 0.01f;
            }

            ViewBody.Rotate(rotaix, 180 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.3f);
        }

        ViewBody.rotation = transform.rotation;
        //isFlip = false;
    }



    void R_JoyStick()
    {        
        if (step < 3) return; // 아직 움직임 단계가 안왔으면 리턴, 

        Vector3 dir = new Vector3(R_x, 0, R_y) * 0.01f; // 패드 xz 값을 받아서 그래도 드론움직임에 적용 

        rotate_aix = Vector3.Cross(dir, Vector3.up); // 회전 축 구하는 함수
        //print(rotate_aix.x+","+ rotate_aix.y +","+ rotate_aix.z);

        if (flymode == Space.Self)
        {
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, ViewBody.eulerAngles.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }
        else if (flymode == Space.World)
        {
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, -transform.rotation.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }

        //방향은 평준화 하고 감도에 따라서 움직임 크기 조정 
        //transform.Translate(dir.normalized * speed * (R_Sense * 0.01f) * Time.deltaTime, flymode); // 비행모드는 headless모드인지아닌지 구분 
        transform.Translate(dir * speed * Time.deltaTime, flymode); // 비행모드는 headless모드인지아닌지 구분 

        if (step == 3)
        {
            StartCoroutine(WaitTime(7.0f,2.0f, 4)); 
            //4 step 으로 넘기고 //  5초 기다렸다가 5step로 넘어감 / + 2초기다려서 클리어 메시지 보여주고  6스텝으로(다음장)! 
        }
    }

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        if (step < 6) return;

        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate(Vector3.up, 0.03f * L_x);
        if (step == 6)
        {
            StartCoroutine(WaitTime(7.0f,2.0f, 7)); 
            //7 step 으로 넘겨 놓구 // 5초기다려다가 8step으로 넘어가기 /  + 2초기다려서 클리어 메시지 보여주고  9스텝으로(다음 스테이지)! 
        }
    }




    IEnumerator WaitTime(float waitTime, int nextStep)
    {
        yield return new WaitForSeconds(waitTime);
        NextStep(nextStep);
    }
    IEnumerator WaitTime(float waitTime,float cleartime , int nextStep)
    {
        NextStep(nextStep);

        yield return new WaitForSeconds(waitTime);
        NextStep(nextStep + 1);
        
        yield return new WaitForSeconds(cleartime); // 행동이 긑나고 다음 클리어 메시지를 보여주고 그 다음 스텝으로 가기 
        NextStep(nextStep + 2);
    }


    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collSenser = true;
        print(collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.isKinematic = false;
        collSenser = false;
        print("bye" + collision.gameObject.name);
    }
}
