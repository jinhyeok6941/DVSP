using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl_CJH : RobotConnector2
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

    //public RigidBody rb;


    public enum VISUAL_STATE
    {
        NORMAL,// 가만히 있는 정지 상태


        HOVERING,

        FORWARD,
        BACK,
        LEFT,
        RIGHT,
        //UP,
        //DOWN,

        FL,
        FR,
        BL,
        BR,

        EndOfType

    }

    VISUAL_STATE view = VISUAL_STATE.NORMAL;


    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
    }

    void Update()
    {
        Start_Stop();//이륙 착륙 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeletMode();
        }

        Debug_tempBytes();
        R_JoyStick();
        ViewingState();
        L_JoyStick();
        Angle_Move();
    }

    void Angle_Move()
    {
       // motion.angleRoll 좌우 기울기. x 축
       // motion.anglePitch 전후 기울기. z 축
       // motion.angleYaw 좌우 회전도. rotation
       // 45도를 각도의 max치라고 가정한다.
       dir = new Vector3(motion.angleRoll , 0 , motion.anglePitch) * 0.01f;
       transform.Translate(dir * speed * Time.deltaTime, flymode);
       ViewBody.rotation = Quaternion.Euler(new Vector3(0, motion.angleYaw, 0));
       //transform.Rotate(0,1,0);
    }

    //string _L_, _R_ = "";
    float L_Sen, R_Sen = 1; // 일단 전달받은 감도 변수를 넣을 자리만 지정해둠 .

    void ViewingState()
    {
        float rot_y = 0;
        if (flymode == Space.Self) { rot_y = 0; }// 머리 있는 모드 
        else if (flymode == Space.World) { rot_y = -transform.rotation.y; }//헤드리스 모드.

        ViewBody.Rotate(dir , 1f);
        //if() 
       //Debug.Log(ViewBody.Quaternion.x + "  ,  " + ViewBody.Quaternion.y + "  ,  " + ViewBody.Quaternion.z);
       //Debug.Log(ViewBody.rotation.x + "  ,  " + ViewBody.rotation.y + "  ,  " + ViewBody.rotation.z);
       //Debug.Log(20 * R_y * 0.01f + "  ,  " + 20 * R_x * 0.01f);
    }//문제점 기체는 회전해서 앞으로 가면 기체의 앞으로 기울어지는데 회전으로 인해 옆으로날라가서 그림이상 

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        ViewBody.Rotate(Vector3.up , 0.03f * L_x);
    }
     Vector3 dir = new Vector3();
    void R_JoyStick()
    {
        dir = new Vector3(R_x,0,R_y) * 0.01f ; // 패드 xz 값을 받아서 그래도 드론움직임에 적용 
        rotate_aix = Vector3.Cross(dir,Vector3.up);

        //ViewBody.rotation = Quaternion.Euler(rotate_aix);  //ViewBody.Rotate(rotate_aix);

        //방향은 평준화 하고 감도에 따라서 움직임 크기 조정 
        //transform.Translate(dir.normalized * speed * (R_Sense * 0.01f) * Time.deltaTime, flymode); // 비행모드는 headless모드인지아닌지 구분 
        transform.Translate(dir * speed * Time.deltaTime, flymode); // 비행모드는 headless모드인지아닌지 구분 
        //R_Sense 는 0부터 100 사이의 값으로 받아오기 때문에 미리 0.01 값을 곱해 놓는다. 
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
}
