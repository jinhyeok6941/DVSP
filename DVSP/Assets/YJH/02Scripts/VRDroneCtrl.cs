using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : MonoBehaviour
{
    //��� ������ 
    public float speed = 10; //  �⺻ ��� ������
    
    public Space flymode = Space.World; //  ��� �����带 HeadLess OnOff ctrl
    int countMode = 0; // flymode ��Ʈ�� �� bool �� �̿밡�� ������ �ϴ� int�� 

    bool isFlying = false; // ��*������ ���� Ȯ�ο� bool �� 
    float pushTime = 0.0f;


    void Start()
    {
        
    }

    void Update()
    {
        L_JoyStick();
        R_JoyStick();

        Start_Stop();//�̷� ���� 

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
            case "TL": //���� 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// ���
                transform.Rotate(Vector3.up, -3f); // �ݽð� ���� ȸ��
                break;
            case "TM": // �� = ��� 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// ���
                break;
            case "TR": // ��� 
                transform.Translate(Vector3.up * speed * Time.deltaTime);// ���
                transform.Rotate(Vector3.up, 3f); // �ð� ���� ȸ�� 
                break;
            case "ML": // ��ȸ�� = �ݽð� ȸ�� 
                transform.Rotate(Vector3.up, -3f); // �ݽð� ���� ȸ��
                break;
            case "CN": // �߽ɿ������� ����
                break;
            case "MR": // ��ȸ�� = �ð� ȸ�� 
                transform.Rotate(Vector3.up, 3f); // �ð� ���� ȸ�� 
                break;
            case "BL": // ����
                transform.Translate(Vector3.down * speed * Time.deltaTime); //�ϰ� 
                transform.Rotate(Vector3.up, -3f); // �ݽð� ���� ȸ��
                break;
            case "BM": // �� = �ϰ�
                transform.Translate(Vector3.down * speed * Time.deltaTime); //�ϰ� 
                break;
            case "BR": // �Ͽ�
                transform.Translate(Vector3.down * speed * Time.deltaTime); //�ϰ� 
                transform.Rotate(Vector3.up, 3f); // �ð� ���� ȸ�� 
                break;
            default:
                break;
        }

        /*
        if (Input.GetKey(KeyCode.I))
        {
            // ���
            //transform.position += Vector3.up * speed * Time.deltaTime; 
            transform.Translate(Vector3.up * speed * Time.deltaTime, flymode);// ���
            //StartFly(transform.position);
        }
        if (Input.GetKey(KeyCode.K))
        {
            //�ϰ�
            //transform.position += Vector3.down * speed * Time.deltaTime;
            transform.Translate(Vector3.down * speed * Time.deltaTime, flymode); //�ϰ� 
        }
        if (Input.GetKey(KeyCode.J))
        {
            //ȸ��
            transform.Rotate(Vector3.up, 3f); // �ð� ���� ȸ�� 
        }
        if (Input.GetKey(KeyCode.L))
        {
            //ȸ��
            transform.Rotate(Vector3.up, -3f); // �ݽð� ���� ȸ�� 
        }*/
    }
    void R_JoyStick()
    {
        Vector3 dir = new Vector3();

        switch (_R_)
        {
            case "TL": //  ����
                dir = Vector3.forward + Vector3.left;
                break;
            case "TM": // �� = ����
                dir = Vector3.forward;
                break;
            case "TR": //  ����
                dir = Vector3.forward + Vector3.right;
                break;
            case "ML": // �� = ���̵�
                dir = Vector3.left;
                break;
            case "CN": // �߽ɿ������� ����
                dir = Vector3.zero;
                break;
            case "MR": // �� = ���̵�
                dir = Vector3.right;
                break;
            case "BL": //  ����
                dir = Vector3.back + Vector3.left;
                break;
            case "BM": // �� = ����
                dir = Vector3.back;
                break;
            case "BR": //  �Ŀ�
                dir = Vector3.back + Vector3.right;
                break;
            default:
                break;
        }

        transform.Translate(dir.normalized * speed * Time.deltaTime, flymode);

        /*
        if (Input.GetKey(KeyCode.W))
        {
            //����
            //transform.position += Vector3.forward * speed * Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //����
            // transform.position += Vector3.back * speed * Time.deltaTime;
            transform.Translate(Vector3.back * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.A))
        {
            //����
            // transform.position += Vector3.left * speed * Time.deltaTime;
            transform.Translate(Vector3.left * speed * Time.deltaTime, flymode);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //������
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
