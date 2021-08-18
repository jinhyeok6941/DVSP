using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl_CJH : RobotConnector2
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

    //public RigidBody rb;


    public enum VISUAL_STATE
    {
        NORMAL,// ������ �ִ� ���� ����


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
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
    }

    void Update()
    {
        Start_Stop();//�̷� ���� 

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
       // motion.angleRoll �¿� ����. x ��
       // motion.anglePitch ���� ����. z ��
       // motion.angleYaw �¿� ȸ����. rotation
       // 45���� ������ maxġ��� �����Ѵ�.
       dir = new Vector3(motion.angleRoll , 0 , motion.anglePitch) * 0.01f;
       transform.Translate(dir * speed * Time.deltaTime, flymode);
       ViewBody.rotation = Quaternion.Euler(new Vector3(0, motion.angleYaw, 0));
       //transform.Rotate(0,1,0);
    }

    //string _L_, _R_ = "";
    float L_Sen, R_Sen = 1; // �ϴ� ���޹��� ���� ������ ���� �ڸ��� �����ص� .

    void ViewingState()
    {
        float rot_y = 0;
        if (flymode == Space.Self) { rot_y = 0; }// �Ӹ� �ִ� ��� 
        else if (flymode == Space.World) { rot_y = -transform.rotation.y; }//��帮�� ���.

        ViewBody.Rotate(dir , 1f);
        //if() 
       //Debug.Log(ViewBody.Quaternion.x + "  ,  " + ViewBody.Quaternion.y + "  ,  " + ViewBody.Quaternion.z);
       //Debug.Log(ViewBody.rotation.x + "  ,  " + ViewBody.rotation.y + "  ,  " + ViewBody.rotation.z);
       //Debug.Log(20 * R_y * 0.01f + "  ,  " + 20 * R_x * 0.01f);
    }//������ ��ü�� ȸ���ؼ� ������ ���� ��ü�� ������ �������µ� ȸ������ ���� �����γ��󰡼� �׸��̻� 

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        ViewBody.Rotate(Vector3.up , 0.03f * L_x);
    }
     Vector3 dir = new Vector3();
    void R_JoyStick()
    {
        dir = new Vector3(R_x,0,R_y) * 0.01f ; // �е� xz ���� �޾Ƽ� �׷��� ��п����ӿ� ���� 
        rotate_aix = Vector3.Cross(dir,Vector3.up);

        //ViewBody.rotation = Quaternion.Euler(rotate_aix);  //ViewBody.Rotate(rotate_aix);

        //������ ����ȭ �ϰ� ������ ���� ������ ũ�� ���� 
        //transform.Translate(dir.normalized * speed * (R_Sense * 0.01f) * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
        transform.Translate(dir * speed * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
        //R_Sense �� 0���� 100 ������ ������ �޾ƿ��� ������ �̸� 0.01 ���� ���� ���´�. 
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
}
