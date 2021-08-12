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

        Debug_tempBytes();
        L_JoyStick();
        R_JoyStick();

        ViewingState();

    }

    //string _L_, _R_ = "";
    float L_Sen, R_Sen = 1; // �ϴ� ���޹��� ���� ������ ���� �ڸ��� �����ص� .

    void ViewingState()
    {
        float rot_y = 0;
        if (flymode == Space.Self) { rot_y = 0; }// �Ӹ� �ִ� ��� 
        else if (flymode == Space.World) { rot_y = -transform.rotation.y; }//��帮�� ���.

        switch (view)
        {
            case VISUAL_STATE.NORMAL:
                ViewBody.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case VISUAL_STATE.HOVERING:
                print("hover");
                ViewBody.localEulerAngles = new Vector3(0, 0, 0);
                break;

            case VISUAL_STATE.FORWARD:
                ViewBody.localEulerAngles = new Vector3(20 , rot_y, 20);
                break;
            case VISUAL_STATE.BACK:
                ViewBody.localEulerAngles = new Vector3(-20, rot_y, 0);
                break;
            case VISUAL_STATE.LEFT:
                ViewBody.localEulerAngles = new Vector3(0, rot_y, 20);
                break;
            case VISUAL_STATE.RIGHT:
                ViewBody.localEulerAngles = new Vector3(0, rot_y, -20);
                break;

            case VISUAL_STATE.FL:
                ViewBody.localEulerAngles = new Vector3(15, rot_y, 15);
                break;
            case VISUAL_STATE.FR:
                ViewBody.localEulerAngles = new Vector3(15, rot_y, -15);
                break;
            case VISUAL_STATE.BL:
                ViewBody.localEulerAngles = new Vector3(-15, rot_y, 15);
                break;
            case VISUAL_STATE.BR:
                ViewBody.localEulerAngles = new Vector3(-15, rot_y, -15);
                break;

            default:
                ViewBody.localEulerAngles = new Vector3(0, 0, 0);
                
                break;
        }

       //ViewBody.rotation = Quaternion.Euler(20 * R_y * 0.01f, 0 , 20 * R_x * 0.01f);
       //Debug.Log(20 * R_y * 0.01f + "  ,  " + 20 * R_x * 0.01f);
    }//������ ��ü�� ȸ���ؼ� ������ ���� ��ü�� ������ �������µ� ȸ������ ���� �����γ��󰡼� �׸��̻� 

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        // switch (L_Joy)
        // {
        //     case "TL": //���� 
        //         transform.Translate(Vector3.up * speed * L_Sense * 0.01f * Time.deltaTime);// ���
        //         transform.Rotate(Vector3.up, -3f); // �ݽð� ���� ȸ��
        //         break;
        //     case "TM": // �� = ��� 
        //         transform.Translate(Vector3.up * speed  * Time.deltaTime);// ���
        //         break;
        //     case "TR": // ��� 
        //         transform.Translate(Vector3.up * speed  * Time.deltaTime);// ���
        //         transform.Rotate(Vector3.up, 3f ); // �ð� ���� ȸ�� 
        //         break;
        //     case "ML": // ��ȸ�� = �ݽð� ȸ�� 
        //         transform.Rotate(Vector3.up, -3f ); // �ݽð� ���� ȸ��
        //         break;
        //     case "CN": // �߽ɿ������� ����
        //         break;
        //     case "MR": // ��ȸ�� = �ð� ȸ�� 
        //         transform.Rotate(Vector3.up, 3f ); // �ð� ���� ȸ�� 
        //         break;
        //     case "BL": // ����
        //         transform.Translate(Vector3.down * speed  * Time.deltaTime); //�ϰ� 
        //         transform.Rotate(Vector3.up, -3f ); // �ݽð� ���� ȸ��
        //         break;
        //     case "BM": // �� = �ϰ�
        //         transform.Translate(Vector3.down * speed  * Time.deltaTime); //�ϰ� 
        //         break;
        //     case "BR": // �Ͽ�
        //         transform.Translate(Vector3.down * speed  * Time.deltaTime); //�ϰ� 
        //         transform.Rotate(Vector3.up, 3f ); // �ð� ���� ȸ�� 
        //         break;
        //     default:
        //         break;
        // }
        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate((Vector3.up * L_x).normalized , 0.03f * L_x);
        Debug.Log(L_x + "  ,  " + (Vector3.up * L_x).normalized);
    }
    void R_JoyStick()
    {
        Vector3 dir = new Vector3();

       /* switch (R_Joy)//joy ������ �Է��ϰ�//���� ���� 
        {
            case "TL": //  ����
                dir = Vector3.forward + Vector3.left;
                view = VISUAL_STATE.FL;
                break;
            case "TM": // �� = ����
                dir = Vector3.forward;
                view = VISUAL_STATE.FORWARD;
                break;
            case "TR": //  ����
                dir = Vector3.forward + Vector3.right;
                view = VISUAL_STATE.FR;
                break;
            case "ML": // �� = ���̵�
                dir = Vector3.left;
                view = VISUAL_STATE.LEFT;
                break;
            case "CN": // �߽ɿ������� ����
                dir = Vector3.zero;
                view = VISUAL_STATE.NORMAL;
                break;
            case "MR": // �� = ���̵�
                dir = Vector3.right;
                view = VISUAL_STATE.RIGHT;
                break;
            case "BL": //  ����
                dir = Vector3.back + Vector3.left;
                view = VISUAL_STATE.BL;
                break;
            case "BM": // �� = ����
                dir = Vector3.back;
                view = VISUAL_STATE.BACK;
                break;
            case "BR": //  �Ŀ�
                dir = Vector3.back + Vector3.right;
                view = VISUAL_STATE.BR;
                break;
            default:
                break;
        }*/

        dir = new Vector3(R_x,0,R_y) * 0.01f ; // �е� xz ���� �޾Ƽ� �׷��� ��п����ӿ� ���� 

        rotate_aix = Vector3.Cross(dir,Vector3.up);

        ViewBody.rotation = Quaternion.Euler(rotate_aix);  //ViewBody.Rotate(rotate_aix);

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
