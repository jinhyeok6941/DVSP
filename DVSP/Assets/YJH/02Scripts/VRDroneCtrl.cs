using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : RobotConnector2
{
    public static VRDroneCtrl instance;

    //드론 움직임 
    public float speed = 10; //  기본 드론 움직임
    
    public Space flymode = Space.World; //  드론 비행모드를 HeadLess OnOff ctrl
    int countMode = 0; // flymode 컨트롤 값 bool 값 이용가능 하지만 일단 int로 

    bool isFlying = false; // 이*착륙을 위한 확인용 bool 값 
    
    //bool isFlip = false; // 플립 입력중에는 안 움직임 확인 용 
    bool isFliping = false; 
    float flipTime;

    bool isSTbtning = false;
    float stbtntime = 0;

    Transform ViewBody;
    Rigidbody rb;

    Vector3 rotate_aix;
    Vector3 rotate_value;

    public Transform joystickL;
    public Transform joystickR;

    public GameObject joypad_Obj;
    JOYPED joypad;

    bool up;//호버링 와리가리 기준 bool값

    bool collSenser = false;

    private void Awake()
    {

        if (this != null) instance = this;
        Connect_Manager();
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
        rb = GetComponent<Rigidbody>(); 
        joypad = joypad_Obj.GetComponent<JOYPED>();//시작시 joypad 오브젝트의 스크립트 가져오기.
    }

    void Start()
    {
        
    }

    void Update()
    {
        Debug_tempBytes();//실시간 정보 받는 함수 업데이트문에 필수 
        // 스타트 스탑을 지금은 부모에서 확인 하구 있음 ! 


        if (Input.GetKeyDown(KeyCode.Space)) // 해드리스 보드 조정 테스트
        {
            SeletMode();
        }
        
        Co_START_STOP(); //  비행 전부터 실행가능 한 함수 시작 중지 

        //--------------------------------//
        if (isFlying == false) return; // 비행중일떄만 아래 조정 가능  
        
        FLIP_Motion(); // 플립 여부 먼저 확인 

        if (isFlip == false) // 플립상태가 아닐떄만 할수 있음!  
        {
            L_JoyStick();
            R_JoyStick();

            Hovering();//호버링 애니매이션
        }

        //---조이패드 ----//

        joypad.JOYSTICK_MOVE(L_x, L_y, R_x, R_y); //조이패드 움직임 함수 !!
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
        print("stst");
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
    }

    

    void FLIP_Motion()
    {
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
            flipTime += Time.deltaTime;

            if (flipTime >= 3.0f) //  버튼을 꾸욱 누르고 있으면 플립 작동 ! 
            {
                StopAllCoroutines();
                StartCoroutine(Flip_H());
                StartCoroutine(Flip_V());
                isFliping = true;
            }
        }
        else
        {
            // isFlip = false;
            isFliping = false;

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

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        //플립중이면 움직이지 않음 
        //if (isFlip) return;

        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate((Vector3.up * L_x).normalized , 0.03f * L_x);
        Debug.Log(L_x + "  ,  " + (Vector3.up * L_x).normalized);
    }
    void R_JoyStick()
    {
        //플립중이면 움직이지 않음 
        //if (isFlip) return;

        Vector3 dir = new Vector3(R_x,0,R_y) * 0.01f ; // 패드 xz 값을 받아서 그래도 드론움직임에 적용 

        rotate_aix = Vector3.Cross(dir,Vector3.up); // 회전 축 구하는 함수
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
    }

    //control 모드 변경으로 해드리스 모드 or 드론 기체 앞의 움직임모드 !
    public void SeletMode()
    {
        countMode++;
        if (countMode % 2 == 0)
        {
            flymode = Space.World;
        }
        else if (countMode % 2 == 1)
        {
            flymode = Space.Self;
            ViewBody.rotation = transform.rotation;
        }


    }
    public void SeletMode(Space mode)
    {
        flymode = mode;        
    }


    public void Hovering()
    {
        if (up)
        {
            ViewBody.localPosition += new Vector3(0,0.001f,0);
        }
        else
        {
            ViewBody.localPosition -= new Vector3(0,0.001f,0);
        }
        //==========
        if (ViewBody.localPosition.y >= 0.3f)
        {
            up = false;
        }
        else if(ViewBody.localPosition.y <= 0)
        {
            up = true;
        }
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
