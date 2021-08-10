using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : MonoBehaviour
{
    //드론 움직임 
    public float speed = 10; //  기본 드론 움직임
    
    public Space flymode = Space.World; //  드론 비행모드를 HeadLess OnOff ctrl
    int countMode = 0; // flymode 컨트롤 값 bool 값 이용가능 하지만 일단 int로 

    bool isFlying = false; // 이*착륙을 위한 확인용 bool 값 
    float pushTime = 0.0f;


    void Start()
    {
        
    }

    void Update()
    {
        L_JoyStick();
        R_JoyStick();

        Start_Stop();//이륙 착륙 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeletMode();
        }
    }

    string _L_, _R_ = "";


    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        switch (_L_)
        {
            case "TL": //상좌 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// 상승
                transform.Rotate(Vector3.up, -3f); // 반시계 방향 회전
                break;
            case "TM": // 상 = 상승 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// 상승
                break;
            case "TR": // 상우 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// 상승
                transform.Rotate(Vector3.up, 3f); // 시계 방향 회전 
                break;
            case "ML": // 좌회전 = 반시계 회전 
                transform.Rotate(Vector3.up, -3f); // 반시계 방향 회전
                break;
            case "CN": // 중심움직이지 않음
                break;
            case "MR": // 우회전 = 시계 회전 
                transform.Rotate(Vector3.up, 3f); // 시계 방향 회전 
                break;
            case "BL": // 하좌
                transform.Translate(Vector3.down * speed * Time.deltaTime); //하강 
                transform.Rotate(Vector3.up, -3f); // 반시계 방향 회전
                break;
            case "BM": // 하 = 하강
                transform.Translate(Vector3.down * speed * Time.deltaTime); //하강 
                break;
            case "BR": // 하우
                transform.Translate(Vector3.down * speed * Time.deltaTime); //하강 
                transform.Rotate(Vector3.up, 3f); // 시계 방향 회전 
                break;
            default:
                break;
        }

        /*
        if (Input.GetKey(KeyCode.I))
        {
            // 상승
            //transform.position += Vector3.up * speed * Time.deltaTime; 
            transform.Translate(Vector3.up * speed * Time.deltaTime, flymode);// 상승
            //StartFly(transform.position);
        }
        if (Input.GetKey(KeyCode.K))
        {
            //하강
            //transform.position += Vector3.down * speed * Time.deltaTime;
            transform.Translate(Vector3.down * speed * Time.deltaTime, flymode); //하강 
        }
        if (Input.GetKey(KeyCode.J))
        {
            //회전
            transform.Rotate(Vector3.up, 3f); // 시계 방향 회전 
        }
        if (Input.GetKey(KeyCode.L))
        {
            //회전
            transform.Rotate(Vector3.up, -3f); // 반시계 방향 회전 
        }*/
    }
    void R_JoyStick()
    {
        Vector3 dir = new Vector3();

        switch (_R_)
        {
            case "TL": //  전좌
                dir = Vector3.forward + Vector3.left;
                break;
            case "TM": // 전 = 전진
                dir = Vector3.forward;
                break;
            case "TR": //  전우
                dir = Vector3.forward + Vector3.right;
                break;
            case "ML": // 좌 = 좌이동
                dir = Vector3.left;
                break;
            case "CN": // 중심움직이지 않음
                dir = Vector3.zero;
                break;
            case "MR": // 우 = 우이동
                dir = Vector3.right;
                break;
            case "BL": //  후좌
                dir = Vector3.back + Vector3.left;
                break;
            case "BM": // 후 = 후진
                dir = Vector3.back;
                break;
            case "BR": //  후우
                dir = Vector3.back + Vector3.right;
                break;
            default:
                break;
        }

        transform.Translate(dir.normalized * speed * Time.deltaTime, flymode);

        /*
        if (Input.GetKey(KeyCode.W))
        {
            //전진
            //transform.position += Vector3.forward * speed * Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //후진
            // transform.position += Vector3.back * speed * Time.deltaTime;
            transform.Translate(Vector3.back * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.A))
        {
            //왼쪽
            // transform.position += Vector3.left * speed * Time.deltaTime;
            transform.Translate(Vector3.left * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //오른쪽
            //transform.position += Vector3.right * speed * Time.deltaTime;
            transform.Translate(Vector3.right * speed * Time.deltaTime, flymode);
        }*/
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
