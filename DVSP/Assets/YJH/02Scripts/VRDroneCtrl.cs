using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : RobotConnector2
{
    //��� ������ 
    public float speed = 10; //  �⺻ ��� ������
    
    public Space flymode = Space.World; //  ��� �����带 HeadLess OnOff ctrl
    int countMode = 0; // flymode ��Ʈ�� �� bool �� �̿밡�� ������ �ϴ� int�� 

    bool isFlying = false; // ��*������ ���� Ȯ�ο� bool �� 
    float pushTime = 0.0f;

    Transform ViewBody;

    Vector3 rotate_aix;
    Vector3 rotate_value;

    public Transform joystickL;
    public Transform joystickR;



    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
    }

    void Start()
    {
        
    }

    void Update()
    {
        Start_Stop();//�̷� ���� 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeletMode();
        }

        Debug_tempBytes();//�ǽð� ���� �޴� �Լ� ������Ʈ���� �ʼ� 

        L_JoyStick();
        R_JoyStick();

        Hovering();//ȣ���� �ִϸ��̼�

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
        Vector3 dir = new Vector3(R_x,0,R_y) * 0.01f ; // �е� xz ���� �޾Ƽ� �׷��� ��п����ӿ� ���� 

        rotate_aix = Vector3.Cross(dir,Vector3.up); // ȸ�� �� ���ϴ� �Լ�
        //print(rotate_aix.x+","+ rotate_aix.y +","+ rotate_aix.z);

        if (flymode == Space.Self) 
        {
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, ViewBody.eulerAngles.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }
        else if (flymode == Space.World) 
        { 
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, -transform.rotation.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }

        //������ ����ȭ �ϰ� ������ ���� ������ ũ�� ���� 
        //transform.Translate(dir.normalized * speed * (R_Sense * 0.01f) * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
        transform.Translate(dir * speed * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
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

        if (pushTime <= 3.0f) return; // ������ �ð��� 3�� �����̸� �۵� ���ϰ� ���� ! 


        //�����尡 �ƴϸ� �̷� �ϰ�
        if (isFlying == false)
        {
            Take_Off(); //1m�� ���� �ö󰡱�
            //isFlying = !isFlying; //�ݴ��! ���� ������ ! 
        }
        //�������̸� �����ϰ� 
        else if(isFlying == true)
        {
            // ������ ���� �� ���� �ϰ�
            
            //isFlying = !isFlying; // �ݴ��! ���� ������ �ƴ�! 
        }
 
    }

    //control ��� �������� �ص帮�� ��� or ��� ��ü ���� �����Ӹ�� !
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
        // ���� ���ó�� �̼��ϰ� ���Ʒ��� �����̴� ��� 
        // �� ��� ���� Updata ���� �̿� �Ǵ°� �ƴ϶� ������ ��� ���� �ϱ� 
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
