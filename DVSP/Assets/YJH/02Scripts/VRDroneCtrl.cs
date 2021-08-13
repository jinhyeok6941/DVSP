using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : RobotConnector2
{
    //드론 움직임 
    public float speed = 10; //  기본 드론 움직임
    
    public Space flymode = Space.World; //  드론 비행모드를 HeadLess OnOff ctrl
    int countMode = 0; // flymode 컨트롤 값 bool 값 이용가능 하지만 일단 int로 

    bool isFlying = false; // 이*착륙을 위한 확인용 bool 값 
    float pushTime = 0.0f;

    Transform ViewBody;

    Vector3 rotate_aix;
    Vector3 rotate_value;

    public Transform joystickL;
    public Transform joystickR;



    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
    }

    void Start()
    {
        
    }

    void Update()
    {
        Start_Stop();//이륙 착륙 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeletMode();
        }

        Debug_tempBytes();//실시간 정보 받는 함수 업데이트문에 필수 

        L_JoyStick();
        R_JoyStick();

        Hovering();//호버링 애니매이션

        joystickL.localPosition = new Vector3(L_x * 0.003f, L_y * 0.003f, -1);
        joystickR.localPosition = new Vector3(R_x * 0.003f, R_y * 0.003f, -1);
    }



    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate((Vector3.up * L_x).normalized , 0.03f * L_x);
        Debug.Log(L_x + "  ,  " + (Vector3.up * L_x).normalized);
    }
    void R_JoyStick()
    {
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

    void Start_Stop()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            pushTime += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            pushTime = 0;
        }

        if (pushTime <= 3.0f) return; // 누르는 시간이 3초 이하이면 작동 안하게 리턴 ! 


        //비행모드가 아니면 이륙 하고
        if (isFlying == false)
        {
            Take_Off(); //1m위 까지 올라가기
            //isFlying = !isFlying; //반대로! 이제 비행중 ! 
        }
        //비행중이면 착륙하고 
        else if(isFlying == true)
        {
            // 접촉이 있을 때 까지 하강
            
            //isFlying = !isFlying; // 반대로! 이제 비행중 아님! 
        }
 
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


    void Take_Off()
    {
        isFlying = true;
        float goal_Y = transform.position.y;
        //for (Vector3 pos = transform.position ; pos == flyLine; pos += Vector3.up * speed * Time.deltaTime)
        //{
        //    transform.position += Vector3.up * speed * Time.deltaTime;
        //    pos = transform.position;

        //}
    }
    void Landing()
    { 
        isFlying = false;
    }

    IEnumerator HoveringAction(bool stop)
    {
        // 실제 드론처럼 미세하게 위아래로 움직이는 모습 
        // 흠 룰루 랄라 Updata 에서 이용 되는게 아니라 구간에 계속 진행 하기 
        yield return new WaitForSeconds(0.1f);

    }
    bool up;

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

        if (ViewBody.localPosition.y >= 0.3f)
        {
            up = false;
        }
        else if(ViewBody.localPosition.y <= 0)
        {
            up = true;
        }
    }
}
